namespace Lottery.Agent.AgentService.Requests.CockFight
{
    public class UpdateDefaultCockFightCompanyBetSettingRequest
    {
        public int BetKindId { get; set; }
        public decimal MainLimitAmountPerFight { get; set; }
        public decimal DrawLimitAmountPerFight { get; set; }
        public decimal LimitNumTicketPerFight { get; set; }
    }
}
