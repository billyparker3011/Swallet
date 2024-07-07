namespace Lottery.Agent.AgentService.Requests.Setting.BetKind
{
    public class BalanceTableNumberDetailRequest
    {
        public List<int> Numbers { get; set; }
        public List<BalanceTableRateRequest> RateValues { get; set; }
    }
}
