namespace Lottery.Core.Models.MatchResult
{
    public class PrizeResultDetailModel
    {
        public int Position { get; set; }
        public bool AllowProcessTicket { get; set; } = true;
        public string Result { get; set; }
    }
}
