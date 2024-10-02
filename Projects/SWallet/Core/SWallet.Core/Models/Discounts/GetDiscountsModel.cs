using HnMicro.Framework.Models;

namespace SWallet.Core.Models.Discounts
{
    public class GetDiscountsModel : QueryAdvance
    {
        public string Keyword { get; set; }
        public bool? IsStatic { get; set; }
        public int? SportKindId { get; set; }
    }
}
