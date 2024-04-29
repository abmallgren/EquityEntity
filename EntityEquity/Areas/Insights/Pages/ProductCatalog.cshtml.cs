using EntityEquity.Data;
using EntityEquity.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Pages.Insights
{
    public class ProductCatalogModel : PageModel
    {
        public string Slug;
        public List<OfferingWithInventoryItem> Items;
        public IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public ProductCatalogModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void OnGet(string slug)
        {
            Slug = slug;
            Items = GetCatalog();
        }
        private List<OfferingWithInventoryItem> GetCatalog()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return (from ii in dbContext.InventoryItems
                               join o in dbContext.Offerings
                                    on ii.InventoryItemId equals o.InventoryItem.InventoryItemId
                               join pom in dbContext.PropertyOfferingMappings
                                   on o.OfferingId equals pom.Offering.OfferingId
                               join p in dbContext.Properties
                                    on pom.Property.PropertyId equals p.PropertyId
                               where p.Slug == Slug
                               select new OfferingWithInventoryItem() { Offering = o, InventoryItem = ii }).ToList();

            }
        }
    }
}
