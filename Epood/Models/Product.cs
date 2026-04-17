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
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; } = 0;
        public bool IsAuction { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime? AuctionEndTime { get; set; }
        public string SellerId { get; set; } = "";
        public ApplicationUser Seller { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Pending;
    }
}
