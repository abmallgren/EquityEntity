using EntityEquity.Data;
using EntityEquity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Areas.Account.Pages
{
    public class OrdersModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        public List<OrderWithInvoices> OrdersWithInvoices;
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterStartDate { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterEndDate { get; set; }
        public OrdersModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
        }
        public async Task OnGet()
        {
            var dbContext = _dbContextFactory.CreateDbContext();
            OrdersWithInvoices = await GetInvoices();
        }
        public async Task OnPost()
        {
            DateTime? startDate = null;
            DateTime? stopDate = null;
            StringValues startDateValue = Request.Form["FilterStartDate"];
            if (!StringValues.IsNullOrEmpty(startDateValue))
                startDate = DateTime.Parse(startDateValue.FirstOrDefault());
            StringValues stopDateValue = Request.Form["FilterStopDate"];
            if (!StringValues.IsNullOrEmpty(stopDateValue))
                stopDate = DateTime.Parse(stopDateValue.FirstOrDefault());
            OrdersWithInvoices = await GetInvoices(startDate, stopDate);
        }
        public async Task<List<OrderWithInvoices>> GetInvoices(DateTime? filterStartDate = null, DateTime? filterStopDate = null)
        {
            if (filterStartDate is null)
            {
                filterStartDate = DateTime.UtcNow.AddDays(-7).Date;
            }
            if (filterStopDate is null)
            {
                filterStopDate = DateTime.UtcNow.AddDays(1).Date;
            }
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                FilterStartDate = filterStartDate.Value;
                FilterEndDate = filterStopDate.Value;

                return (from o in dbContext.Orders.AsEnumerable()
                        select new OrderWithInvoices
                        {
                            Order = o,
                            Invoices = (from i in dbContext.Invoices.Include(p => p.Property).Include(o => o.Order).AsEnumerable()
                                        join ii in dbContext.InvoiceItems.Include(t => t.Invoice)
                                           on i.InvoiceId equals ii.Invoice.InvoiceId
                                        where i.Order.OrderId == o.OrderId &&
                                            i.UserId == _userManager.GetUserId(User) &&
                                            i.ProcessedAt > filterStartDate && i.ProcessedAt < filterStopDate
                                        group new { i, ii } by new { i } into g
                                        orderby g.Key.i.ProcessedAt descending
                                        select new InvoiceWithSum { Invoice = g.Key.i, Amount = g.Sum(a => a.ii.Price * a.ii.Quantity) }).ToList()
                        }).ToList();
            }
        }
    }
}
