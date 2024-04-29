using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Data.CommonDataSets
{
    public class EquityOffers
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private string? _slug;
        public EquityOffers(IDbContextFactory<ApplicationDbContext> dbContextFactory, 
            UserManager<IdentityUser> userManager, 
            string? slug = null)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _slug = slug;
        }
        public int GetUserHoldings(string offerUserId)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var sells = (from et in dbContext.EquityTransactions
                            join p in dbContext.Properties
                                on et.Property.PropertyId equals p.PropertyId
                            where p.Slug == _slug 
                                && et.SellerUserId == offerUserId
                             select et.Shares).FirstOrDefault();
                var buys = (from et in dbContext.EquityTransactions
                           join p in dbContext.Properties
                                on et.Property.PropertyId equals p.PropertyId
                           where p.Slug == _slug
                                && et.BuyerUserId == offerUserId
                           select et.Shares).FirstOrDefault();

                var holdings = new UserHoldings() { Sells = sells, Buys = buys };

                return holdings.Buys - holdings.Sells;
            }
        }

        public List<LiveOffer> GetLiveOffers()
        {
            List<LiveOffer> liveOffers = new List<LiveOffer>();
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var offers = from eo in dbContext.EquityOffers
                             join et in dbContext.EquityTransactions
                                on eo.EquityOfferId equals et.EquityOffer.EquityOfferId into ete
                             from etf in ete.DefaultIfEmpty()
                             join p in dbContext.Properties
                                on eo.Property.PropertyId equals p.PropertyId
                             where p.Slug == _slug
                             group new {eo, etf, p} by new { eo.EquityOfferId, eo.Shares, eo.Price, eo.MustPurchaseAll, eo.UserId } into eog
                             select new { eog.Key.EquityOfferId, eog.Key.UserId, eog.Key.Shares, eog.Key.Price, RemainingShares = (eog.Key.Shares - eog.Sum(e => e.etf.Shares)), eog.Key.MustPurchaseAll };

                foreach (var offer in offers)
                {
                    var balance = GetUserHoldings(offer.UserId);
                    var liveOffer = new LiveOffer()
                    {
                        LiveOfferId = offer.EquityOfferId,
                        Shares = balance > offer.RemainingShares ? offer.RemainingShares : balance,
                        Price = offer.Price,
                        MustPurchaseAll = offer.MustPurchaseAll
                    };
                    if (liveOffer.Shares > 0)
                    { 
                        liveOffers.Add(liveOffer);
                    }
                }
            }
            return liveOffers;
        }
    }
    public class LiveOffer
    {
        public int LiveOfferId { get; set; }
        public int PropertyUrl { get; set; }
        public int Shares { get; set; }
        public decimal Price { get; set; }
        public bool MustPurchaseAll { get; set; }
    }
    public class UserHoldings
    {
        public int Buys;
        public int Sells;
    }
}
