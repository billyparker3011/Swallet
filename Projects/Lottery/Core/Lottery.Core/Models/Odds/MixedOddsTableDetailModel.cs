namespace Lottery.Core.Models.Odds
{
    public class MixedOddsTableDetailModel
    {
        public string Pair { get; set; }
        public decimal Point { get; set; }
        public decimal Payout { get; set; }
        public decimal CompanyPayout { get; set; }
    }
}
