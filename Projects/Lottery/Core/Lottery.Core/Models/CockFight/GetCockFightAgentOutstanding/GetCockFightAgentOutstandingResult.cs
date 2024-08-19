using Lottery.Core.Dtos.CockFight;

namespace Lottery.Core.Models.CockFight.GetCockFightAgentOutstanding
{
    public class GetCockFightAgentOutstandingResult
    {
        public IEnumerable<CockFightAgentOutstandingDto> CockFightAgentOuts { get; set; }
        public long SummaryBetCount { get; set; }
        public decimal SummaryPayout { get; set; }
    }
}
