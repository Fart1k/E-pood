using Epood.Data;
using Epood.Models;
using Epood.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Epood.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ShopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController
            (
            ShopContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        // Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);

            var product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                ImageUrl = vm.ImageUrl,
                Price = vm.Price,
                IsAuction = vm.IsAuction,
                MinPrice = vm.MinPrice,
                AuctionEndTime = vm.AuctionEndTime,
                SellerId = user.Id,
                Status = ProductStatus.Pending
            };

            if (vm.IsAuction && vm.MinPrice <= 0)
            {
                ModelState.AddModelError("", "Auction must have a starting price.");
            }

            if (!vm.IsAuction && vm.Price <= 0)
            {
                ModelState.AddModelError("", "Product must have a price.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // Details
        [HttpGet]
        public IActionResult Details(int id, string? view = null)
        {
            var product = _context.Products
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var bids = _context.Bids
                .Where(x => x.ProductId == id)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new BidHistoryViewModel
                {
                    UserName = x.User.UserName,
                    Amount = x.Amount,
                    CreatedAt = x.CreatedAt,
                })
                .ToList();

            var currentPrice = _context.Bids
                .Where(x => x.ProductId == id)
                .Select(x => (decimal?)x.Amount)
                .Max() ?? product.MinPrice ?? product.Price;    

            var vm = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                ImageUrls = string.IsNullOrEmpty(product.ImageUrl) ? new List<string>() : new List<string> { product.ImageUrl },
                IsAuction = product.IsAuction,
                Price = product.Price,
                MinPrice = product.MinPrice,
                AuctionEndTime = product.AuctionEndTime,
                CurrentPrice = currentPrice,
                SellerId = product.SellerId,

                IsBidHistoryView = view == "history"
            };

            return View(vm);
        }

    }
}
