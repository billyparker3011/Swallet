namespace Lottery.Tools.AdjustOddsService.Models.Payouts
{
    public class PayoutByBetKindAndNumberModel
    {
        public long MatchId { get; set; }
        public int BetKindId { get; set; }
        public int Number { get; set; }
        public decimal Payout { get; set; }

        public override string ToString()
        {
            return $"{MatchId}_{BetKindId}_{Number}";
        }
    }
}
