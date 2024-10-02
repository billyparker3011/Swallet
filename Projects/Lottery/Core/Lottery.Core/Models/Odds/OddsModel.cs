namespace Lottery.Core.Models.Odds
{
    public class OddsModel
    {
        public long Id { get; set; }
        public int BetKindId { get; set; }
        public decimal Buy { get; set; }
        public decimal MinBuy { get; set; }
        public decimal MaxBuy { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxPerNumber { get; set; }
        public long AgentId { get; set; }
    }
}
