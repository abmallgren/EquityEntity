using EntityEquity.Common;
using EntityEquity.Common.Payment;
using EntityEquity.Data;
using EntityEquity.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EntityEquity.Areas.Account.Pages
{
    public class PendingTransactionsModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private HubConnection? hubConnection;
        private CookieBridgeConnection _cookieBridge;
        private AchTransaction _achTransaction;
        private ClaimsPrincipal _user;
        [BindProperty]
        public List<PendingTransaction> PendingTransactions { get; set; }
        public string Cookie;
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterStartDate { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterEndDate { get; set; }
        public PendingTransactionsModel(
            IDbContextFactory<ApplicationDbContext> dbContextFactory, 
            UserManager<IdentityUser> userManager, 
            CookieBridgeConnection cookieBridge, 
            AchTransaction achTransaction,
            ClaimsPrincipal user)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _cookieBridge = cookieBridge;
            _achTransaction = achTransaction;
            _user = user;
        }
        //private string UserId
        //{
        //    get
        //    {
        //        if (TempData["UserId"] is null)
        //        {
        //            TempData["UserId"] = _userManager.GetUserId(User);
        //            TempData.Keep("UserId");
        //        }
        //        return (string)TempData["UserId"];
        //    }
        //}
        public void OnGet()
        {
            Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
            hubConnection = _cookieBridge.GetHubConnection("/entityhub", Cookie);
            PendingTransactions = GetPendingTransactions();
        }
        public void OnPostFilters()
        {
            DateTime? startDate = null;
            DateTime? stopDate = null;
            Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
            StringValues startDateValue = Request.Form["FilterStartDate"];
            if (!StringValues.IsNullOrEmpty(startDateValue))
                startDate = DateTime.Parse(startDateValue.FirstOrDefault());
            StringValues stopDateValue = Request.Form["FilterEndDate"];
            if (!StringValues.IsNullOrEmpty(stopDateValue))
                stopDate = DateTime.Parse(stopDateValue.FirstOrDefault());
            TempData["PendingFilterStartDate"] = startDate;
            TempData["PendingFilterEndDate"] = stopDate;
            TempData.Keep();
            PendingTransactions = GetPendingTransactions(startDate, stopDate);
        }
        public async Task OnPostPendingTable()
        {
            var pendingId = int.Parse(((string)Request.Form["Update"]).Replace("pending", ""));

            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var pendingTransaction = await (from pt in dbContext.PendingTransactions
                                            where pt.PendingTransactionId == pendingId
                                            select pt).FirstOrDefaultAsync();

            var transaction = await _achTransaction.GetTransaction(pendingTransaction.OriginatorId);
            pendingTransaction.Status = transaction.TransactionStatus;
            await dbContext.SaveChangesAsync();
            PendingTransactions = GetPendingTransactions();
        }
        public List<PendingTransaction> GetPendingTransactions(DateTime? filterStartDate = null, DateTime? filterStopDate = null)
        {
            if (filterStartDate is null)
            {
                if (TempData["PendingFilterStartDate"] is not null)
                {
                    filterStartDate = (DateTime)TempData["PendingFilterStartDate"];
                }
                else
                {
                    filterStartDate = DateTime.UtcNow.AddDays(-7).Date;
                }
            }
            if (filterStopDate is null)
            {
                if (TempData["PendingFilterEndDate"] is not null)
                {
                    filterStopDate = (DateTime)TempData["PendingFilterEndDate"];
                }
                else
                { 
                    filterStopDate = DateTime.UtcNow.AddDays(1).Date;
                }
            }
            List<PendingTransaction> transactions = new List<PendingTransaction>();
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                FilterStartDate = filterStartDate.Value;
                FilterEndDate = filterStopDate.Value;

                string userId = _userManager.GetUserId(_user);

                return (from pt in dbContext.PendingTransactions
                        where pt.UserId == userId
                            && (pt.OccurredAt > filterStartDate && pt.OccurredAt < filterStopDate)
                        orderby pt.PendingTransactionId descending
                        select pt).ToList();
            }
        }
    }
}
