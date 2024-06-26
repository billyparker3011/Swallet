namespace Lottery.Core.Models.Announcement.GetAnnouncementByType
{
    public class GetAnnouncementByTypeModel
    {
        public int Type { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool IsAgent { get; set; }
    }
}
