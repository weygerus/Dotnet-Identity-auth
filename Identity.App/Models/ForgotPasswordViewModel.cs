using System.ComponentModel.DataAnnotations;

namespace Identity.App.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "Marco@gmail.com";

        public int Status { get; set;}

        public string Message { get; set;}

        public bool GetPasswordValidationCode(string email)
        {


            return true;
        }
    }

    public struct EmailSetter
    {
        public string From { get; set;}
        public string Subject { get; set;}
        public string EmailBody { get; set;}
    }
}
