namespace Lottery.Core.Models.Setting.ProcessTicket
{
    public class ChannelsForCompletedTicketModel
    {
        public Dictionary<int, List<ChannelsForCompletedTicketDetailModel>> Items { get; set; } = new Dictionary<int, List<ChannelsForCompletedTicketDetailModel>>();

        public static ChannelsForCompletedTicketModel CreateDefault()
        {
            return new ChannelsForCompletedTicketModel();
        }
    }
}
