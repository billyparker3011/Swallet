namespace Lottery.Agent.AgentService.Requests.Odds
{
    public class UpdateOddsItemRequest
    {
        public long Id { get; set; }
        public int BetKindId { get; set; }
        public decimal Buy { get; set; }
        public decimal MinBuy { get; set; }
        public decimal MaxBuy { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxPerNumber { get; set; }
    }
}
