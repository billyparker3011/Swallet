namespace Lottery.Agent.AgentService.Requests.Announcement
{
    public class CreateAnnouncementRequest
    {
        public int Level { get; set; }
        public string Content { get; set; }
        public List<long> ReceivedIds { get; set; } = new List<long>();
    }
}
