namespace Lottery.Core.Models.Odds
{
    public class MixedRateOfOddsValueModel
    {
        public long MatchId { get; set; }
        public Dictionary<int, decimal> TotalRate { get; set; } = new Dictionary<int, decimal>();
    }
}
