namespace Lottery.Core.Models.Ticket.Process;

public class ProcessTicketModel
{
    public int BetKindId { get; set; }
    public long MatchId { get; set; }
    public List<NumberDetailModel> Numbers { get; set; }
    public bool DontAskWhenOddsChange { get; set; }
}
