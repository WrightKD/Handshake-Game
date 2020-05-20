using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HandshakeGame.Database
{
    public class DBConnection: IDBConnection
    {
        ILogger logger;
        public DBConnection(ILogger<DBConnection> logger) {
            this.logger = logger;
        }

        public SqlConnection getConnection() {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=SurveillenceDB;Integrated Security=True";
            SqlConnection cnn = new SqlConnection(connectionString);
            cnn.Open();
            cnn.Close();
            return cnn;
        }
    }
}
