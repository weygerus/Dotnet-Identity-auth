using SendGrid.Helpers.Mail;
using SendGrid;
using Identity.App.Contract.Services;

namespace Identity.App.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task<bool> SetConfirmationEmailSend(string userEmail, string returnUrl)
        {
            var sendGridApiKey = Environment.GetEnvironmentVariable("IDENTITY_APP_KEY_API", EnvironmentVariableTarget.User);

            var messageSubject = "Confirm your account";

            var messageBodyText = "Please confirm your account by clicking <a href=\"" + returnUrl + "\">here</a>";

            var messageBody = @$"

                <div>
                    <div>
                        <h3>Criação de usúario IdentityApp</h3>
                        <p>{messageBodyText}</p>
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
