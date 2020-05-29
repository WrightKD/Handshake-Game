using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace Handshake.Services
{
    public class AdminService
    {

        public IConfiguration Configuration { get; set; }
        public AdminService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Use by injecting admin service into controller, then call : var user = await _adminService.GetUsers();
        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            
            using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser];");
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetUserDetails(int id)
        {
            
            using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<ApplicationUser>($@"SELECT * FROM [ApplicationUser] WHERE [Id] = @{nameof(id)}", new { id });
            }
        }
    }


}
