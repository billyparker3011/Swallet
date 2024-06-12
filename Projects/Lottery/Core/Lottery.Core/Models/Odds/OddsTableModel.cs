using Lottery.Core.Models.Match;

namespace Lottery.Core.Models.Odds
{
    public class OddsTableModel
    {
        public MatchModel Match { get; set; }
        public List<OddsTableDetailModel> Odds { get; set; }
    }
}
