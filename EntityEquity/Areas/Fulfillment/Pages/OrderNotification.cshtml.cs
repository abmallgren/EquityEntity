using EntityEquity.Data;
using EntityEquity.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EntityEquity.Areas.Fulfillment
{
    public class OrderNotificationModel : PageModel
    {
        public List<InvoiceWithInvoiceItems> InvoiceWithInvoiceItems;
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public OrderNotificationModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task OnGet(int orderId, string emailAddress)
        {
            await GetNoticeData(orderId, WebUtility.UrlDecode(emailAddress));
        }
        private async Task GetNoticeData(int orderId, string emailAddress)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            InvoiceWithInvoiceItems = (from i in dbContext.Invoices.Include(p => p.Property).Include(o => o.Order)
                              where i.Order.OrderId == orderId
                                 && i.Property.NotificationEmailAddress == emailAddress
                              select new InvoiceWithInvoiceItems 
                              { 
                                  Invoice = i, 
                                  InvoiceItems = (from ii in dbContext.InvoiceItems
                                                where i.InvoiceId == ii.Invoice.InvoiceId
                                                select ii).ToList() }).ToList();

        }
    }
}
