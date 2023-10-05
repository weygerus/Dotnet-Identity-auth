using Dapper;
using Identity.App.Contract.Repositories;
using Identity.App.Data;
using Identity.App.Models;

namespace Identity.App.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnectionInterface _Connection;

        public AccountRepository(IDbConnectionInterface connection)
        {
            _Connection = connection;
        }

        public bool GetUserByEmail(string userEmail)
        {
            var connection = _Connection.CreateConnection();

            string queryString = $"SELECT * FROM AspNetUsers WHERE UserName in (@Param)";

            var queryResult = connection.Query<ApplicationUser>(queryString, new { Param = userEmail });

            if (queryResult is null)
            {
                return false;
            }

            return true;
        }
    }
}