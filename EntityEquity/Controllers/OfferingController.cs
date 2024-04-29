using Microsoft.AspNetCore.Mvc;
using EntityEquity.Extensions;
using EntityEquity.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Internal;
using EntityEquity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace EntityEquity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferingController : Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<IdentityUser> _userManager;
        public OfferingController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public object Get(string properties = "0", string inventories = "0")
        {
            var context = _dbContext;
            int[] propertyIds;
            int[] inventoryIds;
            InitializeParameters(properties, inventories, out propertyIds, out inventoryIds);
            var offerings = (from o in context.Offerings!.Include(o => o.InventoryItem).Include(o => o.InventoryItem.Inventory)
                             join om in context.OfferingManagers!
                                 on o.OfferingId equals om.Offering.OfferingId
                             join pom in context.PropertyOfferingMappings!
                                 on o.OfferingId equals pom.Offering!.OfferingId
                             join p in context.Properties!
                                 on pom.Property!.PropertyId equals p.PropertyId
                             join pm in context.PropertyManagers!
                                 on pom.Property!.PropertyId equals pm.Property.PropertyId
                             join i in context.Inventories!
                                on o.InventoryItem.Inventory.InventoryId equals i.InventoryId
                             join ii in context.InventoryItems!
                                 on o.InventoryItem.InventoryItemId equals ii.InventoryItemId
                             join im in context.InventoryManagers!
                                 on o.InventoryItem.Inventory.InventoryId equals im.Inventory.InventoryId
                             where om.UserId == _userManager.GetUserId(User)
                                 && om.Role == OfferingManagerRoles.Administrator
                                 && pm.UserId == _userManager.GetUserId(User)
                                 && pm.Role == PropertyManagerRoles.Administrator
                                 && im.UserId == _userManager.GetUserId(User)
                                 && im.Role == InventoryManagerRoles.Administrator
                                 && (propertyIds.Contains(pom.Property!.PropertyId)
                                        || propertyIds.Count() == 0)
                                 && (inventoryIds.Contains(o.InventoryItem.Inventory.InventoryId)
                                        || inventoryIds.Count() == 0)
                                 && o.Deactivated != true
                                 && i.Deactivated != true
                                 && ii.Deactivated != true
                                 && p.Deactivated != true
                             select new { Offering = o, Property = p,
                                 Photos = (from omap in context.OfferingPhotoUrlMappings
                                           join pu in context.PhotoUrls
                                               on omap.PhotoUrl.PhotoUrlId equals pu.PhotoUrlId
                                           where o.OfferingId == omap.Offering.OfferingId
                                           select pu).ToList()
                             }).ToList();

            return offerings;
        }
        private void InitializeParameters(string properties, string inventories, out int[] propertyIds, out int[] inventoryIds)
        {
            if (properties == "0")
            {
                propertyIds = new int[0];
            }
            else
            {
                propertyIds = HttpUtility.UrlDecode(properties).Split(",").ToIntArray();
            }
            if (inventories == "0")
            {
                inventoryIds = new int[0];
            }
            else
            {
                inventoryIds = HttpUtility.UrlDecode(inventories).Split(",").ToIntArray();
            }
        }
    }
}
