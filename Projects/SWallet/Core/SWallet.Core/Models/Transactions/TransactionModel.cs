using SWallet.Core.Enums;

namespace SWallet.Core.Models.Transactions
{
    public class TransactionModel
    {
        public long TransactionId { get; set; }
        public string TransactionName { get; set; }
        public long CustomerId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal OriginAmount { get; set; }
        public decimal Amount { get; set; }
        public TransactionState TransactionState { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
