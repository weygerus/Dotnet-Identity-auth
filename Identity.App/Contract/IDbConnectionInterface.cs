using System.Data;

namespace Identity.App.Data
{
    public interface IDbConnectionInterface
    {
        IDbConnection CreateConnection();
    }
}
