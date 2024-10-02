namespace Lottery.Core.Models.Ticket.Process;

public class ProcessMixedTicketModel
{
    public int BetKindId { get; set; }
    public long MatchId { get; set; }
    public List<int> Numbers { get; set; }
    public Dictionary<int, decimal> Points { get; set; }
}
