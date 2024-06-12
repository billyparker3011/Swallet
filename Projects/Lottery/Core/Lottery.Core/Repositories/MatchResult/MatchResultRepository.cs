using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.MatchResult
{
    public class MatchResultRepository : EntityFrameworkCoreRepository<long, Data.Entities.MatchResult, LotteryContext>, IMatchResultRepository
    {
        public MatchResultRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<List<Data.Entities.MatchResult>> FindByMatchId(long matchId)
        {
            return await FindQueryBy(f => f.MatchId == matchId).ToListAsync();
        }

        public async Task<Data.Entities.MatchResult> FindByMatchIdAndRegionIdAndChannelId(long matchId, int regionId, int channelId)
        {
            return await FindQueryBy(f => f.MatchId == matchId && f.RegionId == regionId && f.ChannelId == channelId).FirstOrDefaultAsync();
        }
    }
}
