using EntityEquity.Data;
using EntityEquity.Data.CommonDataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static EntityEquity.Data.CommonDataSets.EquityOffers;

namespace EntityEquity.Areas.Insights.Pages
{
    public class EquityModel : PageModel
    {
        public string Slug;
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        public EquityModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
        }
        public void OnGet(string slug)
        {
            Slug = slug;
        }
    }
}
