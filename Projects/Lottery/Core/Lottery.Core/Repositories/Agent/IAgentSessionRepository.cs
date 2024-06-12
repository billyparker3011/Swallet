using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities;

namespace Lottery.Core.Repositories.Agent
{
    public interface IAgentSessionRepository : IEntityFrameworkCoreRepository<long, Data.Entities.AgentSession, LotteryContext>
    {
        Task<AgentSession> FindByAgentId(long agentId);
    }
}
