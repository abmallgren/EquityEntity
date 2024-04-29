using EntityEquity.Data;
using EntityEquity.Models.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Pages
{
    public class EquityModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        public EquityModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
        }
        public void OnGet()
        {
        }
        public IEnumerable<NetEquityModel> NetEquity
        {
            get
            {
                return GetNetEquity();
            }
        }
        public IEnumerable<NetEquityModel> GetNetEquity()
        {
            var dbContext = _dbContextFactory.CreateDbContext();
            var boughtEquity = from e in dbContext.EquityTransactions.Include(p => p.Property).AsEnumerable()
                            where e.BuyerUserId == _userManager.GetUserId(User)
                            group new { e } by new { e.Property.PropertyId } into ea
                            select new { ea.Key.PropertyId, SharesBought = ea.Sum(s => s.e.Shares) };
            var soldEquity = from e in dbContext.EquityTransactions.Include(p => p.Property).AsEnumerable()
                                where e.SellerUserId == _userManager.GetUserId(User)
                                group new { e } by new { e.Property.PropertyId } into ea
                                select new { ea.Key.PropertyId, SharesSold = ea.Sum(s => s.e.Shares) };

            var netEquity = (from b in boughtEquity
                                join s in soldEquity
                                    on b.PropertyId equals s.PropertyId into se
                                    from sei in se.DefaultIfEmpty()
                                join p in dbContext.Properties
                                on b.PropertyId equals p.PropertyId
                                group new { b, sei, p } by new { p } into sa
                                select new NetEquityModel() { 
                                    Property = sa.Key.p, 
                                    NetShares = sa.Sum(s => s.b.SharesBought) - sa.Sum(s => s.sei!=null ? s.sei.SharesSold : 0) }).Where(ns => ns.NetShares > 0);

            return netEquity;
        }
    }
}
