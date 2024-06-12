namespace Lottery.Core.Models.Odds
{
    public class AgentMixedOddsModel
    {
        public Dictionary<int, decimal> CompanyOdds { get; set; }   //  Key = SubBetKindId; Value = OddsValue
        public Dictionary<int, decimal> SupermasterOdds { get; set; }
        public Dictionary<int, decimal> MasterOdds { get; set; }
        public Dictionary<int, decimal> AgentOdds { get; set; }
    }
}
