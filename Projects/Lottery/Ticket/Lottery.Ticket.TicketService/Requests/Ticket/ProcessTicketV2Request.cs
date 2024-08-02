namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class ProcessTicketV2Request
{
    public long MatchId { get; set; }
    public bool DontAskWhenOddsChange { get; set; } = true;
    public List<ProcessTicketDetailV2Request> Details { get; set; }
}
