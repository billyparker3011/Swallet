using HnMicro.Framework.Models;

namespace SWallet.ManagerService.Requests
{
    public class GetCustomersOfAgentManagerRequest : QueryAdvance
    {
        public string SearchTerm { get; set; }
        public int? State { get; set; }
    }
}
