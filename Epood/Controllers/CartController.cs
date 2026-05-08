using Epood.Data;
using Epood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

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

        //AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            var existingItem = _shopContext.CartItems
                .FirstOrDefault(x => x.ProductId == productId && x.UserId == user.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Quantity = 1
                };

                _shopContext.CartItems.Add(cartItem);
            }

            await _shopContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        //UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);

            var cartItem = _shopContext.CartItems
                .FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                return NotFound();
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            cartItem.Quantity = quantity;

            await _shopContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);

            var cartItem = _shopContext.CartItems
                .FirstOrDefault(x => x.Id == cartItemId && x.UserId == user.Id);

            if (cartItem == null)
            {
                return NotFound();
            }

            _shopContext.CartItems.Remove(cartItem);
            await _shopContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
