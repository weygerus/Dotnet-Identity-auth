using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Me lembrar")]
        public bool Rememberme { get; set; }

        [Display(Name = "Esqueci minha senha")]
        public string? ForgotPassword { get; set; }
        
        public string? Message { get; set; }
    }
}
