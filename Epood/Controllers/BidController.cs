using Epood.Data;
using Epood.Models;
using Epood.ViewModels;
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

        [Authorize]
        public IActionResult BidHistory(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var product = _context.Products.FirstOrDefault(x => x.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            if (product.SellerId != userId)
            {
                return Forbid();
            }

            var bids = _context.Bids
                .Where(x => x.ProductId ==productId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new BidHistoryViewModel
                {
                    UserName = x.User.UserName,
                    Amount = x.Amount,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            var vm = new BidHistoryPageViewModel
            {
                ProductName = product.Name,
                Bids = bids
            };

            return View("BidHistory" ,vm);
        }
    }
}
