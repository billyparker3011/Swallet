namespace Lottery.Agent.AgentService.Requests.Safeguard
{
    public class ResetSecurityCodeRequest
    {
        public string SecurityCode { get; set; }
        public string ConfirmSecurityCode { get; set; }
    }
}
