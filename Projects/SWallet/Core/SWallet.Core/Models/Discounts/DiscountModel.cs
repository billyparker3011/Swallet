namespace SWallet.Core.Models.Discounts
{
    public class DiscountModel
    {
        public int DiscountId { get; set; }
        public string DiscountName { get; set; }
        public bool IsStatic { get; set; }
        public string Setting { get; set; }
        public bool IsEnabled { get; set; }
    }
}
