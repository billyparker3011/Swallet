namespace Lottery.Core.Models.Payouts
{
    public class CompanyPayoutModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public Dictionary<int, decimal> Payouts { get; set; }
    }
}
