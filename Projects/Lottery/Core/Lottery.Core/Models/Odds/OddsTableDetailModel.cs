namespace Lottery.Core.Models.Odds
{
    public class OddsTableDetailModel
    {
        public int Number { get; set; }
        public decimal Points { get; set; }
        public decimal Payouts { get; set; }
        public decimal OriginValue { get; set; }
        public decimal Rate { get; set; }
    }
}
