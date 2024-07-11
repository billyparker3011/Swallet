namespace Lottery.Agent.AgentService.Requests.Setting.BetKind
{
    public class BalanceTableRateRequest
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal RateValue { get; set; }
        public bool Applied { get; set; }
    }
}
