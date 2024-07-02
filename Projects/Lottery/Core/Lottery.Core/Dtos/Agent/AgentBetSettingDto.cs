namespace Lottery.Core.Dtos.Agent
{
    public class AgentBetSettingDto
    {
        public int BetKindId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BetKindName { get; set; }
        public decimal MinBuy { get; set; }
        public decimal MaxBuy { get; set; }
        public decimal ActualBuy { get; set; }
        public decimal DefaultMinBet { get; set; }
        public decimal ActualMinBet { get; set; }
        public decimal DefaultMaxBet { get; set; }
        public decimal ActualMaxBet { get; set; }
        public decimal DefaultMaxPerNumber { get; set; }
        public decimal ActualMaxPerNumber { get; set; }
        public bool IsDisabled { get; set; }
    }
}
