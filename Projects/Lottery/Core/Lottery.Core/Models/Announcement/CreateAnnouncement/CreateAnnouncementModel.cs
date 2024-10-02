using Lottery.Core.Dtos.Announcement;

namespace Lottery.Core.Models.Announcement.CreateAnnouncement
{
    public class CreateAnnouncementModel
    {
        public int AnnouncementType { get; set; }
        public int AnnouncementLevel { get; set; }
        public string AnnouncementContent { get; set; }
        public List<AnnouncementReceiverDto> AnnouncementReceivers { get; set; } = new List<AnnouncementReceiverDto>();
    }
}
