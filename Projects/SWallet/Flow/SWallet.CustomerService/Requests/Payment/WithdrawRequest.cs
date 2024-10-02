namespace SWallet.CustomerService.Requests.Payment
{
    public class WithdrawRequest
    {
        public long CustomerBankAccountId { get; set; }
        public string PaymentMethodCode { get; set; }
        public decimal Amount { get; set; }
    }
}
