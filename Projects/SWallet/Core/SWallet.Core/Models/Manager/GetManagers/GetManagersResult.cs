using HnMicro.Framework.Responses;

namespace SWallet.Core.Models
{
    public class GetManagersResult
    {
        public IEnumerable<ManagerModel> Managers { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
