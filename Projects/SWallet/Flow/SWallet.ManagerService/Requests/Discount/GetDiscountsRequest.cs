using HnMicro.Framework.Models;

namespace SWallet.ManagerService.Requests.Discount
{
    public class GetDiscountsRequest : QueryAdvance
    {
        public string Keyword { get; set; }
        public bool? IsStatic { get; set; }
        public int? SportKindId { get; set; }
    }
}
