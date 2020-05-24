using HandshakeGame.Models;
using System;
using System.Collections;
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
            string sql = "INSERT dbo.Users (Username, Email, IsAdmin) VALUES (@username, @email, @isAdmin); SELECT SCOPE_IDENTITY();";
            SqlParameter usernameParam = new SqlParameter("@username", item.Username);
            SqlParameter emailParam = new SqlParameter("@email", item.Email);
            SqlParameter isAdminParam = new SqlParameter("@isAdmin", item.IsAdmin ? 1 : 0);
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Add(usernameParam);
            command.Parameters.Add(emailParam);
            command.Parameters.Add(isAdminParam);
            int id = Convert.ToInt32(command.ExecuteScalar());
            User newUser = GetOne(id);
            return newUser;
        }

        public void Delete(User item)
        {
            string sql = "DELETE FROM dbo.Users where UserID = @userID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userID", item.UserID);
            command.ExecuteNonQuery();
        }

        public List<User> GetAll()
        {
            string sql = "SELECT * FROM dbo.Users";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read()) {
                users.Add(readUser(reader));
            }
            reader.Close();
            return users;
        }

        public User readUser(SqlDataReader reader) {
            Position position = null;
            if (!reader.IsDBNull(reader.GetOrdinal("Latitude")) && !reader.IsDBNull(reader.GetOrdinal("Longitude")))
            {
                position = new Position(reader.GetFloat(reader.GetOrdinal("Longitude")), reader.GetFloat(reader.GetOrdinal("Latitude")));
            }
            User user = new User(
                    reader.GetInt32(reader.GetOrdinal("UserID")),
                    reader.GetString(reader.GetOrdinal("Username")),
                    reader.GetString(reader.GetOrdinal("Email")),
                    position,
                    reader.GetInt32(reader.GetOrdinal("Handshakes")),
                    reader.GetBoolean(reader.GetOrdinal("IsAdmin"))
            );
            return user;
        }

        public User GetOne(int id)
        {
            string sql = "SELECT * FROM dbo.Users WHERE UserID = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            User user = readUser(reader);
            reader.Close();
            return user;
        }

        public void Update(User item)
        {
            string sql =
                "UPDATE dbo.Users\n" +
                "SET Username = @username, Email = @email,\n" +
                "Handshakes = @handshakes, IsAdmin = @isAdmin,\n" +
                "Latitude = @latitude, Longitude = @longitude \n" +
                "WHERE UserID = @userID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", item.Username);
            command.Parameters.AddWithValue("@email", item.Email);
            command.Parameters.AddWithValue("@handshakes", item.Handshakes);
            command.Parameters.AddWithValue("@isAdmin", item.IsAdmin ? 1 : 0);
            if (item.Position != null)
            {
                command.Parameters.AddWithValue("@longitude", item.Position.Longitude);
                command.Parameters.AddWithValue("@latitude", item.Position.Latitude);
            }
            else
            {
                command.Parameters.AddWithValue("@longitude", DBNull.Value);
                command.Parameters.AddWithValue("@latitude", DBNull.Value);
            }
            command.Parameters.AddWithValue("@userID", item.UserID);
            command.ExecuteNonQuery();
        }
    }
}
