using Identity.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Identity.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        public SqlConnection CreateConnection()
        {
            var connectionClass = new GetConfiguration();

            var connectionString = connectionClass.GetConnectionString();

            SqlConnection connection = new SqlConnection(connectionString);

            return connection;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/Register")]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    return RedirectToAction("", "");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }            

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _SignInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("", "");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentativa de login inválida");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToAction("", "");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [Route("Account/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var connection = CreateConnection();

            using (connection)
            {
                string queryString = $"SELECT * FROM AspNetUsers WHERE NormalizedUserName in (@Param)";

                var queryResult = connection.Query<ApplicationUser>(queryString, new { Param = model.Email });

                if (queryResult is not null)
                {
                    var validationCode = Convert.ToString(GetRedefinitionValidationCode());

                    if (validationCode.Count() == 4)
                    {
                        var emailSender = new EmailSender();

                        var hasEmailSent = await emailSender.SetEmailSend(model.Email, validationCode);

                        if (hasEmailSent)
                        {
                            var successEmailSentResponse = new ForgotPasswordViewModel()
                            {
                                Status = 0,
                                Message = "E-Mail enviado com sucesso!"
                            };

                            return View(successEmailSentResponse);
                        }
                    }

                    var errorEmailSentResponse = new ForgotPasswordViewModel()
                    {
                        Status = 1,
                        Message = "Erro! E-Mail de recuperação não pode ser enviado"
                    };

                    return View(errorEmailSentResponse);
                }
            }

            return View(model);
        }

        private int GetRedefinitionValidationCode()
        {
            var randomClass = new Random();

            var ValidateCode = Convert.ToInt32(randomClass.Next(1000, 9999));

            return ValidateCode;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }

    public class EmailSender : IEmailSender
    {
        private AuthMessageSenderOptions? Options { get; }

        public EmailSender()
        {

        }

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public async Task<bool> SetEmailSend(string userEmail, string validationCode)
        {
            var sendGridApiKey = Environment.GetEnvironmentVariable("IDENTITY_APP_KEY_API", EnvironmentVariableTarget.User);

            var email = new SendGridMessage()
            {
                From = new EmailAddress("gabrileao38@gmail.com", "Gabriel"),
                Subject = "Email teste",
                PlainTextContent = $"Aqui está o seu código de recuperação de senha: {validationCode}",
                HtmlContent = "Html teste"
            };

            email.AddTo(new EmailAddress(userEmail, "Nome teste"));

            var client = new SendGridClient(sendGridApiKey);

            var emailResponse = await client.SendEmailAsync(email);

            if (emailResponse.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }

    public class AuthMessageSenderOptions
    {
        public string? SendGridUser { get; set; }
        public string? SendGridKey { get; set; }
    }

    public class GetConfiguration
    {
        public string GetApiKey()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetValue<string>("AppSettings:ApiKey");
        }

        public string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DatabaseConnectionString");

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }

            return "";
        }
    }
}