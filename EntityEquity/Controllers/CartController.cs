using EntityEquity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EntityEquity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private UserManager<IdentityUser> _userManager;
        public CartController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public object Get()
        {
            var order = (from o in _dbContext.Orders
                         join oi in _dbContext.OrderItems!
                             on o.OrderId equals oi.Order.OrderId
                         join of in _dbContext.Offerings!
                             on oi.Offering!.OfferingId equals of.OfferingId
                         where o.UserId == _userManager.GetUserId(User)
                             && o.State == OrderState.Incomplete
                             && oi.Quantity > 0
                         select new { OrderItem = oi, Offering = of}).ToList();
            return order;
        }
    }
}
