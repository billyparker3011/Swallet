using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Agent
{
    public class AgentSessionRepository : EntityFrameworkCoreRepository<long, Data.Entities.AgentSession, LotteryContext>, IAgentSessionRepository
    {
        public AgentSessionRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<AgentSession> FindByAgentId(long agentId)
        {
            return await FindQueryBy(f => f.AgentId == agentId).FirstOrDefaultAsync();
        }
    }
}
