using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Data.CommonDataSets
{
    public class Account
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public Account(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public decimal GetBalance(string userId)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return (from le in dbContext.LedgerEntries
                 where le.UserId == userId
                 select le.Amount).Sum();
            }
        }
    }
}
