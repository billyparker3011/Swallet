namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class ProcessTicketDetailV2Request
{
    public int BetKindId { get; set; }
    public int ChannelId { get; set; }
    public List<NumberDetailRequest> Numbers { get; set; }
}
