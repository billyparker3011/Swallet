using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public interface IAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.AgentPositionTaking, LotteryContext>
    {

    }
}
