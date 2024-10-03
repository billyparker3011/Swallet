namespace Lottery.Agent.AgentService.Requests.Setting.BetKind
{
    public class BalanceTableRequest
    {
        public BalanceTableCommonDetailRequest ForCommon { get; set; }
        public BalanceTableNumberDetailRequest ByNumbers { get; set; }
    }
}
