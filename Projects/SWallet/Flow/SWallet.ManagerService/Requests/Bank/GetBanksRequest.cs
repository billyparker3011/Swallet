using HnMicro.Framework.Models;

namespace SWallet.ManagerService.Requests
{
    public class GetBanksRequest : QueryAdvance
    {
        public string SearchName { get; set; }
    }
}
