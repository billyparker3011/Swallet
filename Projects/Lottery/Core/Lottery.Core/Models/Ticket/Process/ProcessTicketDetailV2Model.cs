namespace Lottery.Core.Models.Ticket.Process;

public class ProcessTicketDetailV2Model
{
    public int BetKindId { get; set; }
    public int ChannelId { get; set; }
    public List<NumberDetailModel> Numbers { get; set; }
}