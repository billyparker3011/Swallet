namespace Lottery.Core.Models.Odds
{
    public class OddsByNumberModel
    {
        public int Number { get; set; }
        public List<OddsByBetKindModel> BetKinds { get; set; }
    }
}
