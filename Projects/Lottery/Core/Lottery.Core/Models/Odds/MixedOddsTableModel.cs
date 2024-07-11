using Lottery.Core.Models.Match;

namespace Lottery.Core.Models.Odds
{
    public class MixedOddsTableModel
    {
        public MatchModel Match { get; set; }
        //public Dictionary<int, List<OddsTableDetailModel>> Odds { get; set; }
        public Dictionary<int, string> BetKinds { get; set; }
    }
}
