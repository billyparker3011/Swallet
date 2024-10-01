namespace SWallet.Core.Models.Transactions
{
    public class CompletedTransactionModel
    {
        public long TransactionId { get; set; }
        public decimal Amount { get; set; }
        public int BankId { get; set; }
        public int BankAccountId { get; set; }
    }
}
