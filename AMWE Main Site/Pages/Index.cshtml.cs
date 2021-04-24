using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AMWE_Main_Site.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Добро пожаловать на сайт AMWE!";
        }
    }
}
