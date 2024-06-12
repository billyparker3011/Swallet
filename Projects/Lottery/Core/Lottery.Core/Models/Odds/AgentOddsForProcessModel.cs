namespace Lottery.Core.Models.Odds
{
    public class AgentOddsForProcessModel
    {
        public Dictionary<int, decimal> CompanyOdds { get; set; }
        public Dictionary<int, decimal> SupermasterOdds { get; set; }
        public Dictionary<int, decimal> MasterOdds { get; set; }
        public Dictionary<int, decimal> AgentOdds { get; set; }
    }
}
