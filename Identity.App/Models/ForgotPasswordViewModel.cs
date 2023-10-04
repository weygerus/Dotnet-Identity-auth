using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class ForgotPasswordViewModel
    {

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Name { get; set; }

        public int Status { get; set;}

        public string Message { get; set;}

        public string PasswordRedefinition { get; set; } = "Validar";

        public bool SuccessButton { get; set; }

        public bool GetPasswordValidationCode(string email)
        {
            return true;
        }
    }
}
