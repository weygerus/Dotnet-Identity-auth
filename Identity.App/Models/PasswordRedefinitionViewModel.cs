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
        public string? ConfirmNewPassword { get; set; }
    }
}
