using Dapper;
using Handshake.GameLogic;
using Handshake.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using WebApp.Models;

namespace Handshake.Database
{
    public class UserStore : IUserStore<ApplicationUser>,IUserEmailStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserLoginStore<ApplicationUser>
    {
        private readonly string _connectionString;
        private readonly GameService _gameService;

        public UserStore(IConfiguration configuration, GameService gameService)
        {
            _gameService = gameService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO [ApplicationUser] ([UserName], [NormalizedUserName], [Email],
                [NormalizedEmail], [EmailConfirmed], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled])
                VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.EmailConfirmed)}, @{nameof(ApplicationUser.PasswordHash)},
                @{nameof(ApplicationUser.PhoneNumber)}, @{nameof(ApplicationUser.PhoneNumberConfirmed)}, @{nameof(ApplicationUser.TwoFactorEnabled)});
                SELECT CAST(SCOPE_IDENTITY() as int)", user);

                Player player = _gameService.GetDefaultPlayer();
                player.Id = user.Id;

                var Infected = player.IsInfected ? 1 : 0;
                var Contaminated = player.IsContaminated ? 1 : 0;

                await connection.QuerySingleOrDefaultAsync($@"INSERT INTO [ApplicationPlayer] ([Id], [ScoreCurrent], [ScoreTotal], [Level],
                [ScorePerLevel], [SanitiserCount], [Gold], [MaskCount], [IsContaminated], [IsInfected], [CovidTests])
                VALUES ({player.Id},{player.ScoreCurrent}, {player.ScoreTotal}, {player.Level},
                {player.ScorePerLevel}, {player.SanitiserCount}, {player.Gold},
                {player.MaskCount}, {Contaminated}, {Infected}, {player.CovidTests});");

            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [ApplicationUser] WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [Id] = @{nameof(userId)}", new { userId });
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [NormalizedUserName] = @{nameof(normalizedUserName)}", new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [ApplicationUser] SET
                [UserName] = @{nameof(ApplicationUser.UserName)},
                [NormalizedUserName] = @{nameof(ApplicationUser.NormalizedUserName)},
                [Email] = @{nameof(ApplicationUser.Email)},
                [NormalizedEmail] = @{nameof(ApplicationUser.NormalizedEmail)},
                [EmailConfirmed] = @{nameof(ApplicationUser.EmailConfirmed)},
                [PasswordHash] = @{nameof(ApplicationUser.PasswordHash)},
                [PhoneNumber] = @{nameof(ApplicationUser.PhoneNumber)},
                [PhoneNumberConfirmed] = @{nameof(ApplicationUser.PhoneNumberConfirmed)},
                [TwoFactorEnabled] = @{nameof(ApplicationUser.TwoFactorEnabled)},
                [SerializedRoles] = @{nameof(ApplicationUser.SerializedRoles)}
                WHERE [Id] = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserStore()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser]
                WHERE [NormalizedEmail] = @{nameof(normalizedEmail)}", new { normalizedEmail });
            }
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (!user.Roles.Contains(roleName))
            {
                user.Roles.Add(roleName);
                await UpdateAsync(user, cancellationToken);
            }
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Roles.Contains(roleName));
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user.Roles.Remove(roleName))
                await UpdateAsync(user, cancellationToken);
        }
        #endregion


        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

    }
}
