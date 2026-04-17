namespace Epood.Models
{
    public enum ProductStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; } = 0;

        public string SellerId { get; set; } = "";
        public ApplicationUser Seller { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Pending;
    }
}
