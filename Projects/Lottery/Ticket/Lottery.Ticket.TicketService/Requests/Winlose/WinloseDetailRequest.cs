namespace Lottery.Ticket.TicketService.Requests.Winlose
{
    public class WinloseDetailRequest
    {
        public bool SelectedDraft { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
