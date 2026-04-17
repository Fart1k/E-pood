using Epood.Data;
using Epood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ShopContext _context;
        public AdminController(ShopContext context)
        {
            _context = context;
        }

        public IActionResult PendingProducts()
        {
            var products = _context.Products
                .Where(x => x.Status == ProductStatus.Pending)
                .ToList();

            return View(products);
        }

        public IActionResult Approve(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Status = ProductStatus.Approved;
            _context.SaveChanges();

            return RedirectToAction("PendingProducts");
        }

        public IActionResult Reject(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Status = ProductStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("PendingProducts");
        }
    }
}
