namespace SWallet.Core.Models.Transactions
{
    public class DiscountTransactionModel
    {
        public int DiscountId { get; set; }
        public Guid? ReferenceDiscountDetail { get; set; }
        public long? ReferenceTransactionId { get; set; }
    }
}
