using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Models.Audit;

namespace Lottery.Core.Services.Audit
{
    public interface IAuditService : IScopedDependency
    {
        Task SaveAuditData(AuditParams auditParams);
        Task<GetAuditsByTypeResult> GetAuditsByType(GetAuditsByTypeModel query);
    }
}
