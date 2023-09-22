using Identity.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Identity.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
        }



        //REGISTRO
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



        //LOGIN
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



        //LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToAction("", "");
        }



        //REDEFINIÇÃO DE SENHA
        [HttpGet]
        [Route("Account/ForgotPassword")]
        public IActionResult ForgotPassword(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [Route("Account/ForgotPassword")]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            // TODO: fazer consulta na tabela AspnetUsers para verificar se o email está cadastrado no sistema.

            // TODO: se estiver, (enviar email com o link da pagina de recuperação) OU (Enviar email com token para validar o acesso na tela de recuperação)

            return View();
        }

        [HttpGet]
        [Route("Account/ForgotPassword")]
        public IActionResult PasswordRedefinition(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [Route("Account/ForgotPassword")]
        public IActionResult PasswordRedefinition(PasswordRedefinitionViewModel model)
        {
            // TODO: Refinir senhas no banco de dados.

            // TODO: Ativar Action de login

            return View();
        }



        //REDIRECT
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
}