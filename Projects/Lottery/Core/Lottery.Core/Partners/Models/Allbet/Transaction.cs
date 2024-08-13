namespace Lottery.Core.Partners.Models.Allbet
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public List<BetDetail> Details { get; set; }
        public bool IsRetry { get; set; }
        public string Player { get; set; }
        public long TranId { get; set; }
        public int Type { get; set; }
    }
}
