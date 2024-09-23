namespace SWallet.Core.Models.Payment
{
    public class WithdrawActivityModel
    {
        public long CustomerBankAccountId { get; set; }
        public string PaymentMethodCode { get; set; }
        public decimal Amount { get; set; }
    }
}
