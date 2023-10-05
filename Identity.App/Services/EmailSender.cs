using SendGrid.Helpers.Mail;
using SendGrid;
using Identity.App.Contract.Services;

namespace Identity.App.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task<bool> SetEmailSend(string? userEmail, string messageSubject, string validationCode)
        {
            var sendGridApiKey = Environment.GetEnvironmentVariable("IDENTITY_APP_KEY_API", EnvironmentVariableTarget.User);

            var messageBody = @$"

                <div>
                    <div>
                        <h3>Recuperação de senha</h3>
                        <p>Aqui está o seu código de recuperação de senha:{validationCode}</p>
                        <p>Link para a redefinição:</p>
                        <p>https://localhost:7057/Account/PasswordRedefinition</p>
                    </div>
                </div>

                ";

            var email = new SendGridMessage()
            {
                From = new EmailAddress("gabrileao38@gmail.com", "Identity App"),
                Subject = messageSubject,
                PlainTextContent = null,
                HtmlContent = messageBody
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
