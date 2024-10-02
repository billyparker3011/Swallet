using Lottery.Core.Dtos.BroadCaster;

namespace Lottery.Core.Models.BroadCaster.GetBroadCasterWinlossSummary
{
    public class GetBroadCasterWinlossSummaryResult
    {
        public List<BroadCasterWinlossSummaryDto> BroadCasterWinlossSummaries { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPoint { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal TotalWinLose { get; set; }
        public decimal TotalDraftWinLose { get; set; }
    }
}
