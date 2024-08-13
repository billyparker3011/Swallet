namespace Lottery.Core.Partners.Models.Allbet
{
    public class CasinoAgentPositionTakingModel
    {
        public long Id { get; set; }
        public long AgentId { get; set; }
        public long CABetKindId { get; set; }
        public decimal PositionTaking { get; set; }
    }
}
