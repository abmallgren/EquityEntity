using EntityEquity.Data;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Services
{
    public class DbAppSettings
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public DbAppSettings(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<string?> Get(string key)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            return await (from a in dbContext.AppSettings
                          where a.Key == key
                          select a.Value).FirstOrDefaultAsync();
        }
    }
}
