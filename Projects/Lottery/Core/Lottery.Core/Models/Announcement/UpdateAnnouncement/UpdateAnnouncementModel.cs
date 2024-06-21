namespace Lottery.Core.Models.Announcement.UpdateAnnouncement
{
    public class UpdateAnnouncementModel
    {
        public long AnnouncementId { get; set; }
        public int Level { get; set; }
        public string Content { get; set; }
    }
}
