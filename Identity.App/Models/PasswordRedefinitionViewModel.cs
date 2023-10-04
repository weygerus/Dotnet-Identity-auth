using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class PasswordRedefinitionViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas precisam ser iguais!")]
        public string? ConfirmNewPassword { get; set; }

        public string? ValidationCodeValidate { get; set; }

        public string? ValidateCode { get; set; }

        public string? Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
