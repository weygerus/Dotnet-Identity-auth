using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.App.Pages.Shared
{
    public class LoginModel : PageModel
    {
        [ViewData]
        public string Title { get; set; }

        public void OnGet()
        {
        }
    }
}
