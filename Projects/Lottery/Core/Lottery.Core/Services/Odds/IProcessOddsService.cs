using HnMicro.Core.Scopes;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.Services.Odds
{
    public interface IProcessOddsService : IScopedDependency
    {
        Task<Dictionary<int, OddsStatsModel>> CalculateStats(long matchId, int betKindId);
        Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model);
        Task<Dictionary<int, Dictionary<int, decimal>>> GetRateOfOddsValue(long matchId, List<int> betKindIds);
        void UpdateRateOfOddsValue(long matchId, int betKindId, Dictionary<int, decimal> rate);
    }
}
