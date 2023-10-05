using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Identity.App.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetEmailSend(string? userEmail, string validationCode)
        {
            var sendGridApiKey = Environment.GetEnvironmentVariable("IDENTITY_APP_KEY_API", EnvironmentVariableTarget.User);

            var emailBodyHtmlFilePath = @$"

                <div class="">
                    <div class="">
                        <h3 class="" >Recuperação de senha</h3>
                        <p class="" >Aqui está o seu código de recuperação de senha:{validationCode}</p>
                        <p class="" >Link para a redefinição:</p>
                        <p class="" >https://localhost:7057/Account/PasswordRedefinition</p>
                    </div>
                </div>

                ";

            var emailBodyMessage = @$"{emailBodyHtmlFilePath}";

            var email = new SendGridMessage()
            {
                From = new EmailAddress("gabrileao38@gmail.com", "Identity App"),
                Subject = "Recuperação de senha Identity App",
                PlainTextContent = null,
                HtmlContent = emailBodyMessage
            };

            email.AddTo(new EmailAddress(userEmail, userEmail));

            var client = new SendGridClient(sendGridApiKey);

            var emailResponse = await client.SendEmailAsync(email);

            if (emailResponse.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
