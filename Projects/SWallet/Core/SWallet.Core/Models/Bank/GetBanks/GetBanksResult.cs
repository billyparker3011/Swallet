using HnMicro.Framework.Responses;
using SWallet.Core.Models.Bank;

namespace SWallet.Core.Models
{
    public class GetBanksResult
    {
        public IEnumerable<BankModel> Banks { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
