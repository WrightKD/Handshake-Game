using HandshakeGame.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HandshakeGame.Database.Models
{
    public class Users : IDBModel<User, UserCreate>
    {
        SqlConnection connection;
        public Users(IDBConnection connection)
        {
            this.connection = connection.getConnection();
        }
        public User Create(UserCreate item)
        {
            string sql = "INSERT dbo.Users (Username, Email, IsAdmin) VALUES (@username, @email, @isAdmin)";
            SqlParameter usernameParam = new SqlParameter("@username", item.Username);
            SqlParameter emailParam = new SqlParameter("@email", item.Email);
            SqlParameter isAdminParam = new SqlParameter("@isAdmin", item.IsAdmin);
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add(usernameParam);
            command.Parameters.Add(emailParam);
            command.Parameters.Add(isAdminParam);
            SqlDataReader reader = command.ExecuteReader();


            reader.Read();
            int newID = reader.GetInt32(reader.GetOrdinal("UserID"));
            int newHandshakes = reader.GetInt32(reader.GetOrdinal("Handshakes"));

            User newUser = new User(newID, item.Username, item.Email, null, newHandshakes, item.IsAdmin);
            return newUser;
        }

        public void Delete(User item)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetOne()
        {
            throw new NotImplementedException();
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }
    }
}
