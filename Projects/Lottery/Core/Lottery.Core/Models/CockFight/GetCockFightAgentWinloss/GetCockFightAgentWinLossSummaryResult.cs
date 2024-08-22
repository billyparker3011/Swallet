using Lottery.Core.Dtos.CockFight;

namespace Lottery.Core.Models.CockFight.GetCockFightAgentWinloss
{
    public class GetCockFightAgentWinLossSummaryResult
    {
        public List<CockFightAgentWinlossSummaryDto> CockFightAgentWinlossSummaries { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalWinLose { get; set; }
        public List<TotalCockFightAgentWinLoseInfo> TotalCockFightAgentWinLoseInfo { get; set; }
        public decimal? TotalCompany { get; set; }
    }

    public class TotalCockFightAgentWinLoseInfo
    {
        public decimal TotalWinLose { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalSubTotal { get; set; }
        public int RoleId { get; set; }
    }
}
