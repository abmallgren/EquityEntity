using EntityEquity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public TransactionController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public void Get()
        {
            var dbContext = _dbContextFactory.CreateDbContext();



        }
    }
}
