namespace SWallet.CustomerService.Requests.Transactions
{
    public class GetTransactionsHistoryRequest
    {
        public int? TransactionType { get; set; }
        public int? State { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
    }
}
