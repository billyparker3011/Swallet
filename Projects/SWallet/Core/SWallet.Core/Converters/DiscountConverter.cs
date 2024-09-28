using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
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
                Setting = string.IsNullOrEmpty(discount.Setting) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<DiscountSettingModel>(discount.Setting),
                Description = discount.Description,
                EndDate = discount.EndDate,
                StartDate = discount.StartDate,
                SportKind = discount.SportKindId.HasValue ? discount.SportKindId.Value.ToEnum<SportKind>() : null
            };
        }
    }
}
