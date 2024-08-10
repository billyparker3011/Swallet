namespace Lottery.Core.Models.Setting.ProcessTicket
{
    public class ChannelsForCompletedTicketDetailModel
    {
        public int DayOfWeek { get; set; }
        public List<int> ChannelIds { get; set; } = new List<int>();
    }
}
