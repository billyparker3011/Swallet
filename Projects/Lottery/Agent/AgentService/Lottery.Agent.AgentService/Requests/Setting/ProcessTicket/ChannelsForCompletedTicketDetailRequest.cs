namespace Lottery.Agent.AgentService.Requests.Setting.ProcessTicket
{
    public class ChannelsForCompletedTicketDetailRequest
    {
        public int DayOfWeek { get; set; }
        public List<int> ChannelIds { get; set; } = new List<int>();
    }
}
