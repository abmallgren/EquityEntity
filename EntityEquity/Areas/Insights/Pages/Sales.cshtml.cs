using EntityEquity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Areas.Insights.Pages
{
    public class SalesModel : PageModel
    {
        public string Slug;
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public SalesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void OnGet(string slug)
        {
            Slug = slug;
        }
        public List<InvoiceItem> Items
        {
            get
            {
                return GetData();
            }
        }
        private List<InvoiceItem> GetData()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return (from ii in dbContext.InvoiceItems.Include(ii => ii.Invoice)
                        join i in dbContext.Invoices
                             on ii.Invoice.InvoiceId equals i.InvoiceId
                        where i.Property.Slug == Slug
                        select ii).ToList();
            }
        }
    }
}
