using HnMicro.Framework.Responses;
using Lottery.Core.Dtos.Audit;

namespace Lottery.Core.Models.Audit
{
    public class GetAuditsByTypeResult
    {
        public IEnumerable<AuditDto> Audits { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
