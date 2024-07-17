using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Odds
{
    public interface IProcessOddsService : IScopedDependency
    {
        Task<Dictionary<int, OddsStatsModel>> CalculateStats(long matchId, int betKindId);
        Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model);
        Task<Dictionary<int, Dictionary<int, decimal>>> GetRateOfOddsValue(long matchId, List<int> betKindIds, int noOfNumbers = 100);
        Task UpdateRateOfOddsValue(long matchId, int betKindId, Dictionary<int, decimal> rate);
        Task<List<MixedOddsTableDetailModel>> GetMixedOddsTableDetail(long matchId, int betKindId, int top);
        Task<List<MixedOddsTableRelatedBetKindModel>> GetMixedOddsTableRelatedBetKind(long matchId, int betKindId, int top);
    }
}
