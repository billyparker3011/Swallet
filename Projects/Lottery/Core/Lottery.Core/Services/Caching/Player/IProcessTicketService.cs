using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Outs;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Caching.Player
{
    public interface IProcessTicketService : IScopedDependency
    {
        Task BuildGivenCreditCache(long playerId, decimal credit);
        Task BuildOutsByMatchAndNumbersCache(long playerId, long matchId, Dictionary<int, decimal> pointsByMatchAndNumbers, Dictionary<int, decimal> pointByNumbers);
        Task BuildOutsByMatchCache(long playerId, long matchId, decimal totalOuts);
        Task BuildStatsByMatchBetKindAndNumbers(long matchId, int betKindId, Dictionary<int, decimal> pointByNumbers, Dictionary<int, decimal> payoutByNumbers);

        Task<AgentOddsForProcessModel> GetAgentOdds(int betKindId, long supermasterId, long masterId, long agentId);
        Task<AgentMixedOddsModel> GetAgentMixedOdds(int originBetKindId, List<int> subBetKindIds, long supermasterId, long masterId, long agentId);
        Task<Dictionary<int, decimal>> GetMatchPlayerOddsByBetKindAndNumbers(long playerId, decimal defaultOddsValue, long matchId, int betKindId, List<int> numbers);
        Task<Dictionary<int, decimal>> GetMatchPlayerMixedOddsByBetKind(long playerId, long matchId, int originBetKindId, Dictionary<int, BetSettingModel> subBetKinds);
        Task<(decimal, bool)> GetGivenCredit(long playerId);

        Task<PlayerOutsModel> GetOuts(long playerId, long matchId, List<int> numbers);
        Task<PlayerOutsModel> GetOuts(long playerId, long matchId);
    }
}
