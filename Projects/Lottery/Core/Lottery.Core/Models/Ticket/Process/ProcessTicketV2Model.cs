namespace Lottery.Core.Models.Ticket.Process;

public class ProcessTicketV2Model
{
    public long MatchId { get; set; }
    public bool DontAskWhenOddsChange { get; set; }
    public List<ProcessTicketDetailV2Model> Details { get; set; }
}
