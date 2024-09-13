using HnMicro.Framework.Responses;
using SWallet.Core.Dtos;

namespace SWallet.Core.Models
{
    public class GetBanksResult
    {
        public IEnumerable<BankDto> Banks { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
