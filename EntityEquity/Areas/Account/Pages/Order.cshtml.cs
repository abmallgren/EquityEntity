using EntityEquity.Data;
using EntityEquity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Areas.Account.Pages
{
    public class OrderModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;
        public OrderWithInvoiceItems OrderWithInvoiceItems;
        public string BaseAddress;
        public OrderModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task OnGet(int id)
        {
            var dbContext = _dbContextFactory.CreateDbContext();
            BaseAddress = _configuration.GetValue<string>("BaseAddress");
            OrderWithInvoiceItems = await (from o in dbContext.Orders
                                           where o.OrderId == id
                                           select new OrderWithInvoiceItems 
                                           { 
                                               Order = o,
                                               InvoicesWithInvoiceItems = (from i in dbContext.Invoices.Include(p => p.Property)
                                                                          where i.Order.OrderId == o.OrderId
                                                                           && i.UserId == _userManager.GetUserId(User)
                                                                          select new InvoiceWithInvoiceItems
                                                                          {
                                                                              Invoice = i,
                                                                              InvoiceItems = (from ii in dbContext.InvoiceItems
                                                                                              where ii.Invoice.InvoiceId == i.InvoiceId
                                                                                              select ii).ToList()
                                                                          }).ToList()
                                           }).FirstOrDefaultAsync();
        }
    }

}
