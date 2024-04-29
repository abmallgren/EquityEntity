using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntityEquity.Pages
{
    public class InventoryIndexModel : PageModel
    {
        private UserManager<IdentityUser> _userManager;

        public string UserId;

        public InventoryIndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            UserId = _userManager.GetUserId(User);
        }
    }
}
