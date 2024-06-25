namespace Lottery.Player.PlayerService.Requests.Announcement
{
    public class GetAnnouncementsByTypeRequest
    {
        public int Type { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
