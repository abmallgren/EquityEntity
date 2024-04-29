using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntityEquity.Pages
{
    public class OfferingDetailsModel : PageModel
    {
        public int OfferingId { get; set; }
        public void OnGet(int id)
        {
            OfferingId = id;
        }
    }
}
