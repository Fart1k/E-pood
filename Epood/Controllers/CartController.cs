using Epood.Data;
using Epood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Epood.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ShopContext _shopContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ShopContext shopContext, UserManager<ApplicationUser> userManager)
        {
            _shopContext = shopContext;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var cartItems = _shopContext.CartItems
                .Include(x => x.Product)
                .Where(x => x.UserId == user.Id)
                .ToList();

            return View(cartItems);
        }
        
    }
}
