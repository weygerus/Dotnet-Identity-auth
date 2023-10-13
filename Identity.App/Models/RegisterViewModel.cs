using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas precisam ser iguais!")]
        public string ConfirmPassword { get; set; }

        public string? Message { get; set; }
    }
}
