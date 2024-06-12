using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public interface IAgentAuditRepository : IEntityFrameworkCoreRepository<long, Data.Entities.AgentAudit, LotteryContext>
    {

    }
}
