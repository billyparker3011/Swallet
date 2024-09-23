using HnMicro.Framework.Models;

namespace SWallet.Core.Models
{
    public class GetCustomerOfAgentManagerModel : QueryAdvance
    {
        public string SearchTerm { get; set; }
        public int? State { get; set; }
    }
}
