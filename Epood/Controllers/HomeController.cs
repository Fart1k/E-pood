using Epood.Data;
using Epood.Models;
using Epood.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Epood.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopContext _context;
        public HomeController(ShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Where(x => x.Status == ProductStatus.Approved)
                .Select(p => new ProductListItemViewModel
                {
                    Product = p,
                    CurrentPrice = _context.Bids
                        .Where(b => b.ProductId == p.Id)
                        .Select(b => (decimal?)b.Amount)
                        .Max() ?? p.MinPrice
                })
                .ToList();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
