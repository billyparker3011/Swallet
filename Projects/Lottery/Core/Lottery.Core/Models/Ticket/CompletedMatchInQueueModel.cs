namespace Lottery.Core.Models.Ticket
{
    public class CompletedMatchInQueueModel
    {
        public long MatchId { get; set; }
        public bool IsDraft { get; set; }
        public bool Recalculation { get; set; }
        public int? RegionId { get; set; }
        public int? ChannelId { get; set; }
    }
}
