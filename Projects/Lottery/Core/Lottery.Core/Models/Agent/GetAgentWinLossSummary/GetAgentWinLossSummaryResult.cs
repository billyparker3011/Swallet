using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetAgentWinLossSummary
{
    public class GetAgentWinLossSummaryResult
    {
        public List<AgentWinlossSummaryDto> AgentWinlossSummaries {  get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPoint {  get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalWinLose { get; set; }
        public List<TotalAgentWinLoseInfo> TotalAgentWinLoseInfo { get; set; }
        public decimal? TotalCompany {  get; set; }
    }

    public class TotalAgentWinLoseInfo
    {
        public decimal TotalWinLose { get; set; }
        public decimal TotalCommission { get; set; }
        public int RoleId { get; set; }
    }
}
