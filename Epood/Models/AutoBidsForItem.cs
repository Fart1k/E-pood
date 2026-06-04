namespace Epood.Models
{
    public class AutoBidsForItem
    {
        public Guid AutoSelectorId { get; set; }
        public string UserId { get; set; }
        public string AutoBidsForItemsId { get; set; }
        public string BidListIds { get; set; }
        public decimal MaxAmount { get; set; }
    }
}
