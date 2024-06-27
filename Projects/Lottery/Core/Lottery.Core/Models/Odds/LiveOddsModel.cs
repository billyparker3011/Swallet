namespace Lottery.Core.Models.Odds
{
    public class LiveOddsModel
    {
        public int NoOfRemainingNumbers { get; set; }
        public List<OddsByNumberModel> Odds { get; set; } = new List<OddsByNumberModel>();
    }
}
