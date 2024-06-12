using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Audit;

namespace Lottery.Core.Services.Audit
{
    public interface IAuditService : IScopedDependency
    {
        Task SaveAuditData(AuditParams auditParams);
    }
}
