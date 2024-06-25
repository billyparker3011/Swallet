using Lottery.Core.Dtos.Announcement;

namespace Lottery.Agent.AgentService.Requests.Announcement
{
    public class CreateAnnouncementRequest
    {
        public int Level { get; set; }
        public string Content { get; set; }
        public List<AnnouncementReceiverDto> AnnouncementReceivers { get; set; } = new List<AnnouncementReceiverDto>();
    }
}
