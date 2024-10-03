namespace Lottery.Agent.AgentService.Requests.Setting.ProcessTicket
{
    public class ChannelsForCompletedTicketRequest
    {
        public Dictionary<int, List<ChannelsForCompletedTicketDetailRequest>> Items { get; set; } = new Dictionary<int, List<ChannelsForCompletedTicketDetailRequest>>();
    }
}
