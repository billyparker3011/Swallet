using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Dtos.Announcement
{
    public class AnnouncementDto
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public string Content { get; set; }
        public List<SearchAgentDto> ReceivedAgents { get; set; } = new List<SearchAgentDto>();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
