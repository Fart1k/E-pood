using Microsoft.EntityFrameworkCore;

namespace E_pood.Data
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
    }
}
