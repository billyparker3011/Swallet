namespace Lottery.Tools.AdjustOddsService.Models.Payouts
{
    public class PayoutByMixedAndNumberModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public string PairNumber { get; set; }
        public decimal Payout { get; set; }

        public override string ToString()
        {
            return $"{MatchId}_{BetKindId}_{PairNumber}";
        }
    }
}
