namespace Epood.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAuction { get; set; }
        public decimal Price { get; set; }
        public decimal? MinPrice { get; set; }
        public DateTime? AuctionEndTime { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
