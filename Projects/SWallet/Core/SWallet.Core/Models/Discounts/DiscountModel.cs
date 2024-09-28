using HnMicro.Framework.Enums;

namespace SWallet.Core.Models.Discounts
{
    public class DiscountModel
    {
        public int DiscountId { get; set; }
        public string DiscountName { get; set; }
        public string Description { get; set; }
        public bool IsStatic { get; set; }
        public DiscountSettingModel Setting { get; set; }
        public SportKind? SportKind { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsEnabled { get; set; }
    }
}
