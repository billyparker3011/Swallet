using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetAgentOuts
{
    public class GetAgentOutstandingResult
    {
        public IEnumerable<AgentOutstandingDto> AgentOuts { get; set; }
        public long SummaryBetCount { get; set; }
        public decimal SummaryStake { get; set; }
        public decimal SummaryPayout { get; set; }
    }
}
