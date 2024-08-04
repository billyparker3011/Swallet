namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class ProcessMixedTicketV2Request
{
    public int BetKindId { get; set; }
    public long MatchId { get; set; }
    public List<int> Numbers { get; set; }
    public List<int> ChannelIds { get; set; }
    public Dictionary<int, decimal> Points { get; set; }    //  Key = BetKind; Value = Point
}