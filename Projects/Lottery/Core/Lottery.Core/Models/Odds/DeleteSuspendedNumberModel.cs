namespace Lottery.Core.Models.Odds
{
    public class DeleteSuspendedNumberModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public int Number { get; set; }
    }
}
