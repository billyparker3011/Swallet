namespace Lottery.Agent.AgentService.Requests.Announcement
{
    public class GetAnnouncementsByTypeRequest
    {
        public int Type { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
