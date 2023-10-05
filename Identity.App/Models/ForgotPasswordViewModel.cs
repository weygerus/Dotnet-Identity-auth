using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Message { get; set;}

        public string? ValidateCode { get; set; }

        [Compare("ValidateCode", ErrorMessage = "Código inválido!")]
        public string? ValidateCodeTyped { get; set; }

        public bool HasEmailSent { get; set; }
    }
}
