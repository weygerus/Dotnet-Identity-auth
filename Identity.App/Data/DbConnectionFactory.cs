using System.Data;
using System.Data.SqlClient;

namespace Identity.App.Data
{
    public class DatabaseConnectionFactory : IDbConnectionInterface
    {
        private readonly string? _ConnectionString;

        public DatabaseConnectionFactory(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DatabaseConnectionString");
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection connection = new SqlConnection(_ConnectionString);
            connection.Open();
            return connection;
        }
    }
}