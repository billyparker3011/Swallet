namespace Lottery.Core.Models.Ticket
{
    public class TicketMetadataModel
    {
        public bool IsLive { get; set; }
        public int? Prize { get; set; }
        public int? Position { get; set; }
        public bool AllowProcessTicket { get; set; }
    }
}
