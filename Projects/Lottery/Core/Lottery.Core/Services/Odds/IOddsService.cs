using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Odds
{
    public interface IOddsService : IScopedDependency
    {
        Task<PlayerOddsModel> GetPlayerOddsBy(long playerId, int betKindId);
        Task<List<PlayerOddsModel>> GetMixedOddsBy(long playerId, List<int> betKindIds);
        Task<List<PlayerOddsModel>> GetMixedOddsBy(List<long> playerIds, List<int> betKindIds);
        Task<List<OddsModel>> GetDefaultOdds();
        Task<List<OddsModel>> GetDefaultOddsByBetKind(List<int> betKindIds);
        Task UpdateAgentOdds(List<OddsModel> model, bool updateForCompany = false);
        Task<List<OddsModel>> GetAgentOddsBy(int betKindId, List<long> agentIds);
        Task<List<OddsModel>> GetAgentOddsBy(List<int> betKindIds, List<long> agentIds);

        Task<OddsTableModel> GetOddsTableByBetKind(int betKindId);
        Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model);

        Task<List<OddsByNumberModel>> GetLiveOdds(int betKindId, long playerId);
    }
}
