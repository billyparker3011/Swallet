namespace Lottery.Core.Models.Odds
{
    public class ChangeOddsValueOfOddsTableModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public int Number { get; set; }
        public bool Increment { get; set; }
        public decimal Rate { get; set; } = 1m;
    }
}
