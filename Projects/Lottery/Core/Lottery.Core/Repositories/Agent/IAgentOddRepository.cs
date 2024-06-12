using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public interface IAgentOddRepository : IEntityFrameworkCoreRepository<long, Data.Entities.AgentOdd, LotteryContext>
    {
        Task<List<Data.Entities.AgentOdd>> FindDefaultOdds();
        Task<List<Data.Entities.AgentOdd>> FindDefaultOddsByBetKind(List<int> betKindIds);
    }
}
