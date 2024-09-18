using SWallet.Core.Models.Discounts;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class DiscountConverter
    {
        public static DiscountModel ToDiscountModel(this Discount discount)
        {
            return new DiscountModel
            {
                DiscountId = discount.DiscountId,
                DiscountName = discount.DiscountName,
                IsEnabled = discount.IsEnabled,
                IsStatic = discount.IsStatic,
                Setting = discount.Setting
            };
        }
    }
}
