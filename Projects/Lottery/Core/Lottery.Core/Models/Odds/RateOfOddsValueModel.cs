namespace Lottery.Core.Models.Odds
{
    public class RateOfOddsValueModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public int Number { get; set; }
        public decimal TotalRate { get; set; }
    }
}
