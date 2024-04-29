using EntityEquity.Data;
using EntityEquity.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace EntityEquity.Pages
{
    public class PropIndexModel : PageModel
    {
        public string Slug { get; set; }
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public PropIndexModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void OnGet(string slug)
        {
            Slug = slug;
        }
        public bool IsEquityAvailable
        {
            get 
            {
                return GetIsEquityAvailable();
            }
        }
        public bool GetIsEquityAvailable()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var equityAvailable = (from eo in dbContext.EquityOffers
                                      join et in dbContext.EquityTransactions
                                        on eo.EquityOfferId equals et.EquityOffer.EquityOfferId
                                      where eo.Property.Slug == Slug
                                      group new { eo, et } by new { et.EquityOffer.EquityOfferId } into eoa
                                      select new { eoa.Key.EquityOfferId, RemainingShares = eoa.Sum(e => e.et.Shares) }).FirstOrDefault();
                if (equityAvailable is not null && equityAvailable.RemainingShares > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public Property Property
        {
            get
            {
                return GetProperty();
            }
        }
        private Property GetProperty()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var property = (from p in dbContext.Properties
                                where p.Slug == Slug
                                select p).FirstOrDefault();
                return property;
            }
            
        }
    }
}
