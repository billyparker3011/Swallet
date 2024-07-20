namespace Lottery.Core.Dtos.BroadCaster
{
    public class BroadCasterWinlossSummaryDto
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long BetKindId { get; set; }
        public string BetKindName { get; set; }
        public long BetCount { get; set; }
        public decimal Point { get; set; }
        public decimal Payout { get; set; }
        public decimal WinLose { get; set; }
        public decimal DraftWinLose { get; set; }
    }
}
