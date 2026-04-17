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

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // Details
        [HttpPost]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}
