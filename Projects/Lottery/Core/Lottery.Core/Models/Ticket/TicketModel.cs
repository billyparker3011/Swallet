namespace Lottery.Core.Models.Ticket
{
    public class TicketModel
    {
        public long TicketId { get; set; }
        public bool IsLive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<long> Children { get; set; } = new List<long>();
    }
}
