namespace SWallet.Core.Models.Discounts
{
    public class AddOrUpdateDiscountModel
    {
        public int DiscountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsStatic { get; set; }
        public bool IsEnabled { get; set; }
        public int? SportKindId { get; set; }
        public DateTimeOffset? StartedDate { get; set; }
        public DateTimeOffset? EndedDate { get; set; }
        public DiscountSettingModel Setting { get; set; }
    }
}
