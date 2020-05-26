using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HandshakeGame.Database
{
    public class DBConnection : IDBConnection
    {
        ILogger logger;
        SqlConnection connection;
        public DBConnection(ILogger<DBConnection> logger)
        {
            this.logger = logger;
        }

        ~DBConnection()
        {
            logger.LogInformation("Connection closing");
            if (connection != null)
                connection.Close();
        }

        public SqlConnection getConnection()
        {
            string connectionString = "Server=JACOBR\\SQLEXPRESS;Database=HandshakeDB;Integrated Security=True";
            if (connection == null)
            {
                logger.LogInformation("New connection being created");
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            else
            {
                logger.LogInformation("Existing connection provided");
            }
            return connection;
        }
    }
}
