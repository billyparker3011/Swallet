namespace Lottery.Core.Models.PositionTakings
{
    public class AgentPositionTakingModel
    {
        public long AgentId { get; set; }
        public int BetKindId { get; set; }
        public decimal PositionTaking { get; set; }
    }
}
