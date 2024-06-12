using HnMicro.Core.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums;
using Lottery.Data;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Agent
{
    public class AgentOddRepository : EntityFrameworkCoreRepository<long, AgentOdd, LotteryContext>, IAgentOddRepository
    {
        public AgentOddRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<List<AgentOdd>> FindDefaultOdds()
        {
            return await FindQuery().Include(f => f.Agent).Where(f => f.Agent.RoleId == Role.Company.ToInt()).ToListAsync();
        }

        public async Task<List<AgentOdd>> FindDefaultOddsByBetKind(List<int> betKindIds)
        {
            return await FindQuery().Include(f => f.Agent).Where(f => f.Agent.RoleId == Role.Company.ToInt() && betKindIds.Contains(f.BetKindId)).ToListAsync();
        }
    }
}
