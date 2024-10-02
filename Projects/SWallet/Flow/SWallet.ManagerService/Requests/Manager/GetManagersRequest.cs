using HnMicro.Framework.Models;

namespace SWallet.ManagerService.Requests
{
    public class GetManagersRequest : QueryAdvance
    {
        public string SearchTerm { get; set; }
    }
}
