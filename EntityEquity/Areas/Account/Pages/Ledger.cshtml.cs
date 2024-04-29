using EntityEquity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EntityEquity.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using EntityEquity.Common;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;

namespace EntityEquity.Areas.Account.Pages
{
    public class LedgerModel : PageModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private HubConnection? hubConnection;
        private CookieBridgeConnection _cookieBridge;
        [BindProperty]
        public decimal Balance { get; set; }
        [BindProperty]
        public List<LedgerEntry> LedgerEntries { get; set; }
        public string Cookie;
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterStartDate { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime? FilterEndDate { get; set; }
        public LedgerModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<IdentityUser> userManager, CookieBridgeConnection cookieBridge)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _cookieBridge = cookieBridge;
        }
        public void OnGet()
        {
            Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
            hubConnection = _cookieBridge.GetHubConnection("/entityhub", Cookie);
            hubConnection.On("UpdateBalance", () =>
            {
                Balance = GetBalance();
                LedgerEntries = GetLedgerEntries();
            });
            Balance = GetBalance();
            LedgerEntries = GetLedgerEntries();
        }
        public void OnPost()
        {
            DateTime? startDate = null;
            DateTime? stopDate = null;
            Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
            StringValues startDateValue = Request.Form["FilterStartDate"];
            if (!StringValues.IsNullOrEmpty(startDateValue))
                startDate = DateTime.Parse(startDateValue.FirstOrDefault());
            StringValues stopDateValue = Request.Form["FilterStopDate"];
            if (!StringValues.IsNullOrEmpty(stopDateValue))
                stopDate = DateTime.Parse(stopDateValue.FirstOrDefault());
            Balance = GetBalance();
            LedgerEntries = GetLedgerEntries(startDate, stopDate);
        }
        public decimal GetBalance()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return (from l in dbContext.LedgerEntries.AsEnumerable()
                        where l.UserId == _userManager.GetUserId(User)
                        select l.Amount).Sum();
            }
        }
        public List<LedgerEntry> GetLedgerEntries(DateTime? filterStartDate = null, DateTime? filterStopDate = null)
        {
            if (filterStartDate is null)
            {
                filterStartDate = DateTime.UtcNow.AddDays(-7).Date;
            }
            if (filterStopDate is null)
            {
                filterStopDate = DateTime.UtcNow.AddDays(1).Date;
            }
            List<LedgerEntry> entries = new List<LedgerEntry>();
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                FilterStartDate = filterStartDate.Value;
                FilterEndDate = filterStopDate.Value;

                return (from l in dbContext.LedgerEntries
                        where l.UserId == _userManager.GetUserId(User)
                            && (l.OccurredAt > filterStartDate && l.OccurredAt < filterStopDate)
                        select l).ToList();
            }
        }
    }
}
