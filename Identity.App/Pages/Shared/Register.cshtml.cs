using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Identity.App.Pages.Shared
{
    public class ResgisterModel : PageModel
    {
        [ViewData]
        public string Title { get; set; }

        public void OnGet()
        {
        }
    }
}
