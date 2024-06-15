using HnMicro.Core.Scopes;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.Match.ChangeState;
using Lottery.Core.Models.Match.CreateMatch;
using Lottery.Core.Models.MatchResult;

namespace Lottery.Core.Services.Match
{
    public interface IMatchService : IScopedDependency
    {
        Task CreateMatch(CreateOrUpdateMatchModel model);
        Task UpdateMatch(CreateOrUpdateMatchModel model);
        Task ChangeState(long matchId, ChangeStateModel model);
        Task<MatchModel> GetRunningMatch(bool inCache = true);
        Task UpdateMatchResults(MatchResultModel model);
        Task<ResultModel> ResultsByKickoff(DateTime kickOffTime);
        Task<List<MatchModel>> GetMatches(int top = 30, bool displayResult = false);
        Task<MatchModel> GetMatchById(long matchId);
        Task OnOffProcessTicketOfChannel(OnOffProcessTicketOfChannelModel model);
    }
}
