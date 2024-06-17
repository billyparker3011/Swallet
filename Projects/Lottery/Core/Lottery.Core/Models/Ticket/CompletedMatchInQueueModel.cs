namespace Lottery.Core.Models.Ticket
{
    public class CompletedMatchInQueueModel
    {
        public long MatchId { get; set; }
        public bool IsDraft { get; set; }
        public bool Recalculation { get; set; }
    }
}
