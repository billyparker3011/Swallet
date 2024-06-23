using HnMicro.Core.Scopes;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;

namespace Lottery.Core.Services.Match
{
    public interface IRunningMatchService : IScopedDependency
    {
        Task<MatchModel> GetRunningMatch(bool inCache = true);
        List<PrizeResultModel> DeserializeResults(string results);
        string SerializeResults(List<PrizeResultModel> results);
    }
}
