using Identity.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.App.Pages.Shared
{
    public class ForgotPasswordModel : PageModel
    {
        [ViewData]
        public string Title { get; set; }

        public void OnGet()
        {
        }
    }
}
