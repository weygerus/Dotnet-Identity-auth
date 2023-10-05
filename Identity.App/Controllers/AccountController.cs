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
using Identity.App.Data;
using Identity.App.Services;

namespace Identity.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly IDbConnectionInterface _Connection;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IDbConnectionInterface connection)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _Connection = connection;
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
        public async Task<IActionResult> ForgotPassword(string message, bool hasEmailSent, string validationCode, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            ViewData["Message"] = message;

            ViewData["HasEmailSent"] = hasEmailSent;

            if (!string.IsNullOrEmpty(validationCode))
            {
                ViewData["ValidationCode"] = validationCode;
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var connection = _Connection.CreateConnection();

            using (connection)
            {
                string queryString = $"SELECT * FROM AspNetUsers WHERE UserName in (@Param)";

                var queryResult = connection.Query<ApplicationUser>(queryString, new { Param = model.Email });

                if (queryResult is null)
                {
                    return View(model);
                }

                var validationCode = GetRedefinitionValidationCode();

                var emailSender = new EmailSender();

                var hasEmailSent = await emailSender.SetEmailSend(model.Email, validationCode);

                if (!hasEmailSent)
                {
                    var errorEmailSentResponse = new ForgotPasswordViewModel()
                    {
                        Status = 1,
                        Message = "Erro! E-Mail de recuperação não pode ser enviado"
                    };

                    return View(errorEmailSentResponse);
                }

                var successEmailSentResponse = new ForgotPasswordViewModel()
                {
                    Status = 0,
                    Message = "E-Mail enviado com sucesso! Por favor valide o código de recuperação enviado: ",
                    HasEmailSent = true
                };

                return RedirectToAction("ForgotPassword", new { message = successEmailSentResponse.Message, hasEmailSent = successEmailSentResponse.HasEmailSent, validationCode = validationCode});
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordRedefinition(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [AutoValidateAntiforgeryToken]
        [Route("Account/PasswordRedefinition")]
        public async Task<IActionResult> PasswordRedefinition(string emailUserName, string newPassword)
        {
            var connection = _Connection.CreateConnection();

            using (connection)
            {
                var queryParam = newPassword;

                var queryStringQuote = @"
                                    {
                                         <status_option>
                                        | WITH <set_option> [ ,... ]
                                        | <cryptographic_credential_option>
                                    }
                ";

                var onOff = "{ ON | OFF }";

                var sqlQuery = 
                        @$"

                ALTER LOGIN UserName {emailUserName}
                {queryStringQuote}

                <set_option> ::=
                WITH PasswordHash = '{queryParam}' | hashed_password HASHED
                [
                OLD_PASSWORD = 'oldpassword'
                | <password_option> [<password_option> ]
                ]
                | DEFAULT_DATABASE = database
                | DEFAULT_LANGUAGE = language
                | NAME = login_name
                | CHECK_POLICY = {onOff}
                | CHECK_EXPIRATION = {onOff}
                | CREDENTIAL = credential_name
                | NO CREDENTIAL

                        ";

                var queryResult = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(sqlQuery, queryParam);

                var model = new PasswordRedefinitionViewModel();

                if (queryResult is not null)
                {
                    model.IsSuccess = true;

                    model.Message = "Senha Alterada com sucesso!";

                    return View();
                }

                model.IsSuccess = true;

                model.Message = "Erro interno! Infelizmente, o sistema não pode alterar sua senha!";

                return View();
            }
        }

        private string GetRedefinitionValidationCode()
        {
            var randomClass = new Random();

            var ValidateCode = Convert.ToInt32(randomClass.Next(1000, 9999));

            return Convert.ToString(ValidateCode);
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
    }
}