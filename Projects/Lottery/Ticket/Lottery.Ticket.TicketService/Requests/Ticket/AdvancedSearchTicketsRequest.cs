using HnMicro.Framework.Models;

namespace Lottery.Ticket.TicketService.Requests.Ticket;

public class AdvancedSearchTicketsRequest : QueryAdvance
{
    public long MatchId { get; set; }
    public List<long> TicketIds { get; set; } = new List<long>();
    public List<string> Username { get; set; } = new List<string>();
    public List<int> BetKindIds { get; set; } = new List<int>();
    public int RegionId { get; set; }
    public int ChannelId { get; set; }
    public List<int> ChooseNumbers { get; set; } = new List<int>();
    public List<int> States { get; set; } = new List<int>();
    public List<bool> LiveStates { get; set; } = new List<bool>();
    public List<int> Prizes { get; set; } = new List<int>();
    public List<int> Positions { get; set; } = new List<int>();
}