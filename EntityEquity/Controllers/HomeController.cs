using EntityEquity.Data.CommonDataSets;
using EntityEquity.Data;
using EntityEquity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using EntityEquity.Data.Models;

namespace EntityEquity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, 
            IDbContextFactory<ApplicationDbContext> dbContextFactory, 
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            HomeIndexModel indexModel = new();
            indexModel.Performances = new();

            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var shareholders = (from et in dbContext.EquityTransactions.Include(p => p.Property).AsEnumerable()
                                    where (from pom in dbContext.PropertyOfferingMappings
                                           join of in dbContext.Offerings
                                               on pom.Offering.OfferingId equals of.OfferingId
                                           join oi in dbContext.OrderItems
                                               on of.OfferingId equals oi.Offering.OfferingId
                                           select pom.Property.PropertyId).Contains(et.Property.PropertyId)
                                    group new { et } by new { et.Property.PropertyId, PropertyName = et.Property.Name, PropertyShares = et.Property.Shares, et.BuyerUserId } into bet
                                    where (bet.Sum(e => e.et.Shares) -
                                        dbContext.EquityTransactions.AsEnumerable().Where(ets => ets.Property.PropertyId == bet.Key.PropertyId
                                            && ets.SellerUserId == bet.Key.BuyerUserId)
                                        .Select(ets => ets.Shares)
                                        .Sum()) > 0

                                    select new Shareholder
                                    {
                                        PropertyId = bet.Key.PropertyId,
                                        PropertyName = bet.Key.PropertyName,
                                        PropertyShares = bet.Key.PropertyShares,
                                        UserId = bet.Key.BuyerUserId,
                                        Shares = bet.Sum(e => e.et.Shares) - (from etsi in dbContext.EquityTransactions
                                                                              where etsi.Property.PropertyId == bet.Key.PropertyId
                                                                              && etsi.SellerUserId == bet.Key.BuyerUserId
                                                                              select etsi.Shares).Sum()
                                    }).ToList();

                var properties = (from p in dbContext.Properties.AsEnumerable()
                                 join pm in dbContext.PropertyManagers.Include(pm => pm.Property)
                                                 on p.PropertyId equals pm.Property.PropertyId
                                 where (shareholders.Where(sh => sh.PropertyId == p.PropertyId
                                                      && sh.UserId == _userManager.GetUserId(User)).Any()
                                                  || p.OwnerUserId == _userManager.GetUserId(User)
                                                  || (pm.UserId == _userManager.GetUserId(User)
                                                     && pm.Role == PropertyManagerRoles.Administrator))
                                    && p.Deactivated != true
                                 select p).AsEnumerable();

                var invoiceItems = (from p in properties
                                    join oi in dbContext.OrderItems.Include(oi => oi.Property).Include(oi => oi.Order)
                                                  on p.PropertyId equals oi.Property.PropertyId into oia
                                              from oid in oia.DefaultIfEmpty()
                                              join i in dbContext.Invoices.Include(i => i.Order)
                                                  on oid?.Order.OrderId equals i.Order.OrderId into ia
                                              from iad in ia.DefaultIfEmpty()
                                              join ii in dbContext.InvoiceItems
                                                  on iad?.InvoiceId equals ii.Invoice.InvoiceId into iia
                                              from iiad in iia.DefaultIfEmpty()
                                              where (iad == null || iad.ProcessedAt > DateTime.Now.AddDays(-7))
                                              select new { p, iiad }).Distinct();
                var performanceIndicators = (from ii in invoiceItems
                                             group new { ii } by new { ii.p.PropertyId } into iia
                                             select new { iia.Key.PropertyId, WeeklySalesAmount = iia.Sum(i => i.ii.iiad != null ? i.ii.iiad.Price : 0) }).ToList();
                var knownProperties = from pi in performanceIndicators
                                    select pi.PropertyId;
                var allProperties = from ap in properties
                                    where !knownProperties.Contains(ap.PropertyId)
                                    select new { ap };
                foreach (var prop in allProperties)
                {
                    performanceIndicators.Add(new { PropertyId = prop.ap.PropertyId, WeeklySalesAmount = (decimal)0 });
                }
                performanceIndicators = performanceIndicators.OrderBy(p => p.PropertyId).ToList();

                foreach (var performanceIndicator in performanceIndicators)
                {
                    Property property = (from p in dbContext.Properties
                                         where p.PropertyId == performanceIndicator.PropertyId
                                         select p).FirstOrDefault();

                    var equityOffers = new EquityOffers(_dbContextFactory, _userManager, property.Slug);
                    var ownedShares = equityOffers.GetUserHoldings(_userManager.GetUserId(User));

                    PropertyPerformance performance = new()
                    {
                        Property = property,
                        SharesOwned = ownedShares,
                        SalesInLastWeek = performanceIndicator.WeeklySalesAmount
                    };

                    indexModel.Performances.Add(performance);
                }
            }

            return View(indexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}