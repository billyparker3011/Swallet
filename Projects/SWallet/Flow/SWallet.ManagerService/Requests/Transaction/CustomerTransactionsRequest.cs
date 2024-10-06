namespace SWallet.ManagerService.Requests
{
    public class CustomerTransactionsRequest
    {
        public int? TransactionType { get; set; }
        public int? State { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }
}
