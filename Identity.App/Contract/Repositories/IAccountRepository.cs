namespace Identity.App.Contract.Repositories
{
    public interface IAccountRepository 
    {
        public bool GetUserByEmail(string userEmail);
    }
}
