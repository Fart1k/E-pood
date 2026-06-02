using Epood.Data;
using Epood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Epood.Controllers
{
    [Authorize]
    public class BidController : Controller
    {
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BidController(ShopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(int productId, decimal amount)
        {
            var user = await _userManager.GetUserAsync(User);

            var product = await _context.Products.FindAsync(productId); 

            if (product == null)
            {
                return NotFound();
            }

            if (!product.IsAuction)
            {
                TempData["Error"] = "This product is not an auction.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            

            var highestBid = _context.Bids.Where(x => x.ProductId == productId).OrderByDescending(x => x.Amount).FirstOrDefault();

            if (highestBid != null && amount <= highestBid.Amount)
            {
                TempData["Error"] = "Bid must be higher than the current highest bid.";

                return RedirectToAction("Details", "Product", new { id = productId });
            }

            if (highestBid == null && amount < product.MinPrice)
            {
                TempData["Error"] = "Bid must be at least the minimum price.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var bid = new Bid
            {
                ProductId = productId,
                UserId = user.Id,
                Amount = amount
            };

            _context.Bids.Add(bid);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Bid submitted.";

            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}
