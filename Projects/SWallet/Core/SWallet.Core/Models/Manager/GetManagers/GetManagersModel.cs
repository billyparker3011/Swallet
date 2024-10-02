using HnMicro.Framework.Models;

namespace SWallet.Core.Models
{
    public class GetManagersModel : QueryAdvance
    {
        public long? ManagerId { get; set; }
        public string SearchTerm { get; set; }
        public int? State { get; set; }
    }
}
