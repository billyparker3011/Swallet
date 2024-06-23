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
        Task ChangeState(long matchId, ChangeStateModel model);
        Task<ResultModel> ResultsByKickoff(DateTime kickOffTime);
        Task<List<MatchModel>> GetMatches(int top = 30, bool displayResult = false);
        Task<MatchModel> GetMatchById(long matchId);

        Task StartStopProcessTicket(StartStopProcessTicketModel model);
        Task StartStopProcessTicketByPosition(StartStopProcessTicketByPositionModel model);
        Task StartStopLive(StartStopLiveModel model);
        Task UpdateResult(UpdateResultModel model);
        Task UpdateRunningMatch();
    }
}
