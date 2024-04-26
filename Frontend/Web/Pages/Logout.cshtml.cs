using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserData;

namespace MyApp.Namespace
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            UserId.user_id = -1;
            return RedirectToPage("Index");
        }
    }
}
