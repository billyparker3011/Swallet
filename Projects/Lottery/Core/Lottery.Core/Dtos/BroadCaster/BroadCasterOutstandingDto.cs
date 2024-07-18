namespace Lottery.Core.Dtos.BroadCaster
{
    public class BroadCasterOutstandingDto
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long BetKindId { get; set; }
        public string BetKindName { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalStake { get; set; }
        public decimal TotalPayout { get; set; }
    }
}
