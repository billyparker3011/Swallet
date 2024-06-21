namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class RefundRejectTicketsRequest
{
    public List<long> TicketIds { get; set; } = new List<long>();
}
