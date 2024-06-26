using HnMicro.Core.Scopes;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Match
{
    public interface IRunningMatchService : IScopedDependency
    {
        Task<MatchModel> GetRunningMatch(bool inCache = true);
        List<PrizeResultModel> DeserializeResults(string results);
        string SerializeResults(List<PrizeResultModel> results);
        decimal GetLiveOdds(int betKindId, MatchModel match, decimal defaultOddsValue);
        List<OddsByNumberModel> GetOddsByPlayerForNorthern(long playerId, List<PlayerOddsModel> playerOdds, Dictionary<int, Dictionary<int, decimal>> rateOfOddsValue, MatchModel runningMatch);
    }
}
