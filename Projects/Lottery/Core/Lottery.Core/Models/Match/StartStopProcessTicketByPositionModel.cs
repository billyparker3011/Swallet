namespace Lottery.Core.Models.Match;

public class StartStopProcessTicketByPositionModel
{
    public long MatchId { get; set; }
    public int RegionId { get; set; }
    public int ChannelId { get; set; }
    public int PrizeId { get; set; }
    public int Position { get; set; }
}