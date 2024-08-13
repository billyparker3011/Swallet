namespace Lottery.Agent.AgentService.Requests.CockFight
{
    public class UpdateCockFightAgentBetSettingRequest
    {
        public int BetKindId { get; set; }
        public decimal MainLimitAmountPerFight { get; set; }
        public decimal DefaultMaxMainLimitAmountPerFight { get; set; }
        public decimal DrawLimitAmountPerFight { get; set; }
        public decimal DefaultMaxDrawLimitAmountPerFight { get; set; }
        public decimal LimitNumTicketPerFight { get; set; }
        public decimal DefaultMaxLimitNumTicketPerFight { get; set; }
    }
}
