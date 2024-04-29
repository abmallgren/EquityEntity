using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EntityEquity.Data.CommonDataSets
{
    public class Orders
    {
        private IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private UserManager<IdentityUser> _userManager;
        private ClaimsPrincipal _user;
        public Orders(IDbContextFactory<ApplicationDbContext> dbContextFactory, 
            UserManager<IdentityUser> userManager,
            ClaimsPrincipal user)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _user = user;
        }
        public Order? GetIncompleteOrder()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                return (from o in dbContext.Orders
                        where o.UserId == _userManager.GetUserId(_user)
                            && o.State == OrderState.Incomplete
                        select o).FirstOrDefault();
            }
        }
    }
}
