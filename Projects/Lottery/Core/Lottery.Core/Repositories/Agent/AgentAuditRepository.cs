using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public class AgentAuditRepository : EntityFrameworkCoreRepository<long, Data.Entities.AgentAudit, LotteryContext>, IAgentAuditRepository
    {
        public AgentAuditRepository(LotteryContext context) : base(context)
        {
        }
    }
}
