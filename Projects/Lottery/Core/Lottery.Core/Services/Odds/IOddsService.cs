using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Odds
{
    public interface IOddsService : IScopedDependency
    {
        Task<List<PlayerOddsModel>> GetMixedOddsBy(long playerId, List<int> betKindIds);
        Task<List<PlayerOddsModel>> GetMixedOddsBy(List<long> playerIds, List<int> betKindIds);
        Task<List<OddsModel>> GetDefaultOdds();
        Task<List<OddsModel>> GetDefaultOddsByBetKind(List<int> betKindIds);
        Task UpdateAgentOdds(List<OddsModel> model, bool updateForCompany = false);
        Task<List<OddsModel>> GetAgentOddsBy(int betKindId, List<long> agentIds);
        Task<List<OddsModel>> GetAgentOddsBy(List<int> betKindIds, List<long> agentIds);

        Task<OddsTableModel> GetOddsTableByBetKind(int betKindId);
        Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model);
        Task<MixedOddsTableModel> GetMixedOddsTableByBetKind(int betKindId);

        Task<List<OddsByNumberModel>> GetInitialOdds(long playerId, int betKindId);
        Task<Dictionary<long, LiveOddsModel>> GetLiveOdds(List<long> playerIds, int betKindId, long matchId, int regionId, int channelId);
    }
}
