using EntityEquity.Data;
using EntityEquity.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace EntityEquity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropOfferingController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private UserManager<IdentityUser> _userManager;
        public PropOfferingController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        public List<OfferingWithOrderItem> Get(int propertyId = 0, string? searchPhrase = "")
        {
            using (var dbContext = _dbContext)
            {
                var offerings = (from o in dbContext.Offerings!.Include(o => o.InventoryItem).Include(o => o.InventoryItem.Inventory)
                                 join oi in dbContext.OrderItems!
                                    .Include(oi => oi.Order)
                                    .Include(oi => oi.Property)
                                    on new { OfferingId = o.OfferingId, UserId = _userManager.GetUserId(User), OrderState = OrderState.Incomplete  } 
                                        equals new { OfferingId = oi.Offering.OfferingId, UserId = oi.Order.UserId, OrderState = oi.Order.State }
                                        into oie
                                 from orit in oie.DefaultIfEmpty()
                                 join pom in dbContext.PropertyOfferingMappings!
                                    on o.OfferingId equals pom.Offering!.OfferingId
                                 where (pom.Property!.PropertyId == propertyId || 
                                    (propertyId == 0 && (o.Name.Contains(searchPhrase)
                                       || o.Description.Contains(searchPhrase))))
                                       && o.Deactivated != true
                                       && o.InventoryItem.Deactivated != true
                                       && o.InventoryItem.Inventory.Deactivated != null
                                       && orit.Property.Deactivated != true
                                 select new OfferingWithOrderItem { 
                                     Offering = o, 
                                     OrderItem = orit,
                                     Property = pom.Property,
                                     Photos = (from omap in dbContext.OfferingPhotoUrlMappings
                                                join pu in dbContext.PhotoUrls
                                                    on omap.PhotoUrl.PhotoUrlId equals pu.PhotoUrlId
                                                where o.OfferingId == omap.Offering.OfferingId
                                                select pu).ToList()
                                 }).ToList();
                return offerings;
            }
        }

    }
}
