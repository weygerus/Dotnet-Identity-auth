using System.Data;
using System.Data.SqlClient;

namespace Identity.App.Data
{
    public class DatabaseConnectionFactory : IDbConnectionInterface
    {
        public IDbConnection CreateConnection()
        {
            var connectionString = GetConnectionString();

            IDbConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        private string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DatabaseConnectionString");

            return connectionString;
        }
    }
}