using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.MatchResult
{
    public interface IMatchResultRepository : IEntityFrameworkCoreRepository<long, Data.Entities.MatchResult, LotteryContext>
    {
        Task<List<Data.Entities.MatchResult>> FindByMatchId(long matchId);
        Task<Data.Entities.MatchResult> FindByMatchIdAndRegionIdAndChannelId(long matchId, int regionId, int channelId);
    }
}
