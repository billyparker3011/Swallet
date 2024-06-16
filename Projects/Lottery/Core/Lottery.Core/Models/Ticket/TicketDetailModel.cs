namespace Lottery.Core.Models.Ticket
{
    public class TicketDetailModel : BaseTicketDetailModel
    {
        public long PlayerId { get; set; }
        public string Username { get; set; }
    }
}
