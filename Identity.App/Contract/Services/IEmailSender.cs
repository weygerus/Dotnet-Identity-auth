namespace Identity.App.Contract.Services
{
    public interface IEmailSender
    {
        public Task<bool> SetEmailSend(string? userEmail, string messageSubject, string validationCode);
    }
}
