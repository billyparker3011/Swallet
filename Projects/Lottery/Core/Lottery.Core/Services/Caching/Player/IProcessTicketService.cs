using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Outs;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Caching.Player
{
    public interface IProcessTicketService : IScopedDependency
    {
        Task BuildGivenCreditCache(long playerId, decimal credit);
        Task BuildPointsByMatchBetKindAndNumbersCache(long playerId, long matchId, Dictionary<int, Dictionary<int, decimal>> pointsByBetKindAndNumbers);

        Task BuildOutsByMatchCache(long playerId, long matchId, decimal totalOuts);
        Task BuildStatsByMatchBetKindAndNumbers(long matchId, int betKindId, Dictionary<int, decimal> pointByNumbers, Dictionary<int, decimal> payoutByNumbers, Dictionary<int, decimal> companyPayoutByNumbers);
        Task BuildMixedStatsByMatch(long matchId, Dictionary<int, Dictionary<string, decimal>> pointByPair, Dictionary<int, Dictionary<string, decimal>> payoutByPair, Dictionary<int, Dictionary<string, decimal>> realPayoutByPair);

        Task UpdateOutsByMatchCache(Dictionary<long, Dictionary<long, decimal>> downOuts);
        Task UpdatePointsByMatchBetKindNumbersCache(Dictionary<long, Dictionary<long, Dictionary<int, Dictionary<int, decimal>>>> points);
        Task UpdateStatsByMatchBetKindAndNumbers(Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> outs, Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> points);

        Task<AgentOddsForProcessModel> GetAgentOdds(int betKindId, long supermasterId, long masterId, long agentId, int noOfNumbers = 100);
        Task<AgentMixedOddsModel> GetAgentMixedOdds(int originBetKindId, List<int> subBetKindIds, long supermasterId, long masterId, long agentId);
        Task<Dictionary<int, decimal>> GetMatchPlayerOddsByBetKindAndNumbers(long playerId, decimal defaultOddsValue, long matchId, int betKindId, List<int> numbers, int noOfNumbers = 100);
        Task<Dictionary<int, decimal>> GetMatchPlayerMixedOddsByBetKind(long playerId, long matchId, Dictionary<int, BetSettingModel> subBetKinds);
        Task<(decimal, bool)> GetGivenCredit(long playerId);

        Task<PlayerOutsModel> GetOuts(long playerId, long matchId, int betKindId, List<int> numbers);
        Task<MixedPlayerOutsModel> GetMixedOdds(long playerId, long matchId, List<int> betKindIds, List<int> numbers);

        Task<PlayerOutsModel> GetOuts(long playerId, long matchId);
    }
}
