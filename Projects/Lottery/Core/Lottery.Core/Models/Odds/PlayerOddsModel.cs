namespace Lottery.Core.Models.Odds
{
    public class PlayerOddsModel
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public int BetKindId { get; set; }
        public decimal Buy { get; set; }
    }
}
