namespace Lottery.Core.Models.Announcement.GetUnreadAnnouncements
{
    public class GetUnreadAnnouncementsResult
    {
        public List<UnreadAnnouncement> UnreadAnnouncements { get; set; }
        public int TotalUnreadAnnouncement => UnreadAnnouncements != null && UnreadAnnouncements.Any() ? UnreadAnnouncements.Sum(x => x.Quantity) : 0;
    }

    public class UnreadAnnouncement
    {
        public int Type { get; set; }
        public int Quantity { get; set; }
    }
}
