using HnMicro.Core.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Match
{
    public class MatchRepository : EntityFrameworkCoreRepository<long, Data.Entities.Match, LotteryContext>, IMatchRepository
    {
        public MatchRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<Data.Entities.Match> FindByMatchCode(string matchCode)
        {
            return (await FindByAsync(f => f.MatchCode == matchCode)).FirstOrDefault();
        }

        public async Task<bool> HaveRunningOrSuspendedMatch()
        {
            return await FindQueryBy(f => f.MatchState == MatchState.Running.ToInt() || f.MatchState == MatchState.Suspended.ToInt()).AnyAsync();
        }

        public async Task<Data.Entities.Match> GetRunningMatch()
        {
            return await FindQueryBy(f => f.MatchState == MatchState.Running.ToInt()).FirstOrDefaultAsync();
        }

        public async Task<Data.Entities.Match> GetMatchByKickoffTime(DateTime kickOffTime)
        {
            return await FindQueryBy(f => f.KickOffTime.Date == kickOffTime.Date).Include(f => f.MatchResults).OrderByDescending(f => f.KickOffTime).FirstOrDefaultAsync();
        }
    }
}
