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

            await CheckAutoBids(productId);

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

        public async Task CheckAutoBids(int productId)
        {
            var autoEntry = _context.AutoBidsForItems
                .FirstOrDefault(x => x.AutoBidsForItemsId == productId.ToString());

            if (autoEntry == null)
            {
                return;
            }

            var highestBid = _context.Bids
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.Amount)
                .FirstOrDefault();

            if (highestBid == null)
                return;

            var autoUsers = _context.AutoBidsForItems
                .Where(x => x.AutoBidsForItemsId == productId.ToString())
                .Where(x => x.MaxAmount > highestBid.Amount)
                .Where(x => !string.IsNullOrEmpty(x.UserId))
                .OrderByDescending(x => x.MaxAmount)
                .ToList();

            var ordered = autoUsers
                .OrderByDescending(x => x.MaxAmount)
                .ToList();
            if (highestBid == null)
            {
                return;
            }

            foreach (var auto in ordered)
            {
                if (auto.MaxAmount <= highestBid.Amount)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(auto.UserId))
                {
                    continue;
                }

                if (auto.UserId == highestBid.UserId)
                {
                    continue;
                }

                var newAmount = Math.Min(auto.MaxAmount, highestBid.Amount + 1);

                var autoBid = new Bid
                {
                    ProductId = productId,
                    UserId = auto.UserId,
                    Amount = newAmount,
                    IsAutomatic = true
                };

                _context.Bids.Add(autoBid);

                highestBid = autoBid;

                Console.WriteLine($"Autobid triggered for user {auto.UserId}, max {auto.MaxAmount}");
            }

            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public IActionResult EnableAutoBid(int productId, decimal maxAmount)
        {
            var userId = _userManager.GetUserId(User);

            var existing = _context.AutoBidsForItems
                .FirstOrDefault(x => x.AutoBidsForItemsId == productId.ToString() && x.UserId == userId);

            if (existing != null)
            {
                existing.MaxAmount = maxAmount;
            }

            else
            {
                var entry = new AutoBidsForItem
                {
                    AutoSelectorId = Guid.NewGuid(),
                    AutoBidsForItemsId = productId.ToString(),
                    UserId = userId,
                    MaxAmount = maxAmount,
                    BidListIds = ""
                };

                _context.AutoBidsForItems.Add(entry);
            }

            _context.SaveChanges();

            TempData["Success"] = "Auto bid enabled.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}