using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Models
{
    public class UserCreate
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }

        public UserCreate(string email, string username, bool isAdmin = false) {
            Email = email;
            Username = username;
            IsAdmin = isAdmin;
        }
    }
}
