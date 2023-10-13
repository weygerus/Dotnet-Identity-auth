namespace Identity.App.Contract.Services
{
    public interface IEmailSender
    {
        public Task<bool> SetConfirmationEmailSend(string userEmail, string returnUrl);
    }
}
