namespace Lottery.Core.Models.Odds
{
    public class AddSuspendedNumbersModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public List<int> Numbers { get; set; }
    }
}
