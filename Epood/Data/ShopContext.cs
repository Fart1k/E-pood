using Microsoft.EntityFrameworkCore;

namespace Epood.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
    }
}
