using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Match
{
    public interface IMatchRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Match, LotteryContext>
    {
        Task<Data.Entities.Match> FindByMatchCode(string matchCode);
        Task<bool> HaveRunningOrSuspendedMatch();
        Task<Data.Entities.Match> GetRunningMatch();
        Task<Data.Entities.Match> GetMatchByKickoffTime(DateTime kickOffTime);
        Task<Data.Entities.Match> GetLatestMatch();
    }
}
