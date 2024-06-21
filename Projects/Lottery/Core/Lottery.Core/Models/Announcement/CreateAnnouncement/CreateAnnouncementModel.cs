namespace Lottery.Core.Models.Announcement.CreateAnnouncement
{
    public class CreateAnnouncementModel
    {
        public int AnnouncementType { get; set; }
        public int AnnouncementLevel { get; set; }
        public string AnnouncementContent { get; set; }
        public List<long> ReceivedIds { get; set; } = new List<long>();
    }
}
