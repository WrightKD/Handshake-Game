using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string SerializedRoles
        {
            get
            {
                if (_roles == null || _roles.Count == 0)
                    return null;
                return JsonConvert.SerializeObject(Roles);
            }
            set
            {
                _roles = JsonConvert.DeserializeObject<IList<string>>(value) ?? new List<string>();
            }
        }

        private IList<string> _roles;
        public IList<string> Roles 
        {

            get
            {
                _roles = _roles ?? new List<string>();
                return _roles;
            }

            set
            {
                _roles = value;
            }
        
        }
    }
}
