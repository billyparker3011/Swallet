namespace Lottery.Core.Dtos.Agent
{
    public class AgentPositionTakingDto
    {
        public int BetKindId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BetKindName { get; set; }
        public decimal DefaultPositionTaking { get; set; }
        public decimal ActualPositionTaking { get; set; }
        public bool IsDisabled { get; set; }
    }
}
