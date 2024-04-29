using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntityEquity.Pages.Insights
{
    public class IndexModel : PageModel
    {
        public string Slug { get; set; }
        public void OnGet(string slug)
        {
            Slug = slug;
        }
    }
}
