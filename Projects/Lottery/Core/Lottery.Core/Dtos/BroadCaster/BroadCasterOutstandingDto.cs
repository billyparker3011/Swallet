namespace Lottery.Core.Dtos.BroadCaster
{
    public class BroadCasterOutstandingDto
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BetKindId { get; set; }
        public string BetKindName { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalStake { get; set; }
        public decimal TotalPayout { get; set; }
    }
}
