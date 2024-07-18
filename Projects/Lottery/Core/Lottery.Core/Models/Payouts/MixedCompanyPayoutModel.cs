namespace Lottery.Core.Models.Payouts
{
    public class MixedCompanyPayoutModel
    {
        public long MatchId { get; set; }
        public Dictionary<int, Dictionary<string, decimal>> Payouts { get; set; }
    }
}
