namespace Lottery.Core.Models.Ticket.Process;

public class ProcessMixedTicketV2Model
{
    public int BetKindId { get; set; }
    public long MatchId { get; set; }
    public List<int> Numbers { get; set; }
    public List<int> ChannelIds { get; set; }
    public Dictionary<int, decimal> Points { get; set; }
}