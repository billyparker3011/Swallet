using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public class AgentPositionTakingRepository : EntityFrameworkCoreRepository<long, Data.Entities.AgentPositionTaking, LotteryContext>, IAgentPositionTakingRepository
    {
        public AgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
