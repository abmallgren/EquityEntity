using EntityEquity.Data;
using EntityEquity.Data.CommonDataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Areas.Prep.Pages
{
    public class EquityModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        public string Slug;
        public int Balance;
        public Property Property;
        public EquityModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
        }
        public async Task OnGet(string slug)
        {
            Slug = slug;
            Balance = GetBalance();
            Property = await GetProperty(slug);
        }
        private int GetBalance()
        {
            EquityOffers dataset = new(_dbContextFactory, _userManager, Slug);
            var holdings = dataset.GetUserHoldings(_userManager.GetUserId(User));
            return holdings;
        }
        private async Task<Property> GetProperty(string slug)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var property = await (from p in dbContext.Properties
                                where p.Slug == slug
                                select p).FirstOrDefaultAsync();

                return property;
            }
        }
    }
}
