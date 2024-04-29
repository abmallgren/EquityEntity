using EntityEquity.Data;
using EntityEquity.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityEquity.Controllers
{
    public class ProController : Controller
    {
        IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public ProController(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public IActionResult Index()
        {
            var model = new SearchResultsModel()
            {
                Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"],
                SearchPhrase = ""
            };
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SearchResults(string searchPhrase)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var model = new SearchResultsModel()
                {
                    Cookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"],
                    SearchPhrase = searchPhrase
                };
                return View(model);
            }
        }
    }
}
