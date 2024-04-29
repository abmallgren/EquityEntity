using EntityEquity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Miscellaneous : ControllerBase
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public Miscellaneous(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task Get()
        {
            StreamReader reader = new StreamReader(Request.Body);
            string json = await reader.ReadToEndAsync();

            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            Temporary temp = new()
            {
                Content = json
            };
            dbContext.Temporaries.Add(temp);
            await dbContext.SaveChangesAsync();
        }
        public async Task Post()
        {
            StreamReader reader = new StreamReader(Request.Body);
            string json = await reader.ReadToEndAsync();

            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            Temporary temp = new()
            {
                Content = "POST - " + json
            };
            dbContext.Temporaries.Add(temp);
            await dbContext.SaveChangesAsync();
        }
    }
}
