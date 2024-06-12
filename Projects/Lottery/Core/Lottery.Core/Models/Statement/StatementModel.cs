namespace Lottery.Core.Models.Statement
{
    public class StatementModel
    {
        public long MatchId { get; set; }
        public string MatchCode { get; set; }
        public DateTime Kickofftime { get; set; }
        public object TotalPoint { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal? TotalWinlose { get; set; }
    }
}
