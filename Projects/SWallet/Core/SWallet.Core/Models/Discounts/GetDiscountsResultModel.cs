using HnMicro.Framework.Models;

namespace SWallet.Core.Models.Discounts
{
    public class GetDiscountsResultModel : BaseResult
    {
        public List<DiscountModel> Discounts { get; set; }
    }
}
