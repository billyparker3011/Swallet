namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class RefundRejectTicketsByNumbersRequest : RefundRejectTicketsRequest
{
    public List<int> Numbers { get; set; } = new List<int>();
}