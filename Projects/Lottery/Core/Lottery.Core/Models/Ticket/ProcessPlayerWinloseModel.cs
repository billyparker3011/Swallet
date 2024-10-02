namespace Lottery.Core.Models.Ticket
{
    public class ProcessPlayerWinloseModel
    {
        public long PlayerId { get; set; }
        public string PartOfWinloseMainKey { get; set; }
        public decimal WinloseValue { get; set; }
        public int SportKindId { get; set; }
    }
}
