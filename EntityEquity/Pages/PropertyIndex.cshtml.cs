using EntityEquity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Pages
{
    public class PropertyIndexModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _contextFactory;
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;

        public string UserId;
        public List<Property> Properties;

        public PropertyIndexModel(IDbContextFactory<ApplicationDbContext> contextFactory, 
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _configuration = configuration;

            UserId = "";
            Properties = new();
        }

        public void OnGet()
        {
            UserId = _userManager.GetUserId(User);
        }
    }
}
