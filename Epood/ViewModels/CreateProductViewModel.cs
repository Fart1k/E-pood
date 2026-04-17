namespace Epood.ViewModels
{
    public class CreateProductViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public Decimal Price { get; set; }
        public bool IsAuction { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime? AuctionEndTime { get; set; }
    }
}
