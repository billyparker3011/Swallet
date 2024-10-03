namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class GetAgentWinLossSearchRequest
    {
        public bool SelectedDraft { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
