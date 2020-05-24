using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Handshakes { get; set; }
        public Position Position;
        public bool IsAdmin;
        public User(int id, string username, string email, Position position, int handshakes, bool isAdmin)
        {
            UserID = id;
            Username = username;
            Email = email;
            Position = position;
            Handshakes = handshakes;
            IsAdmin = isAdmin;
        }

    }
}
