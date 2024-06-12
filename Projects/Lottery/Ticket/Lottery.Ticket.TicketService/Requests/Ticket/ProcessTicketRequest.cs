namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class ProcessTicketRequest
{
    public int BetKindId { get; set; }
    public long MatchId { get; set; }
    public List<NumberDetailRequest> Numbers { get; set; }
    public bool DontAskWhenOddsChange { get; set; } = true;
}
