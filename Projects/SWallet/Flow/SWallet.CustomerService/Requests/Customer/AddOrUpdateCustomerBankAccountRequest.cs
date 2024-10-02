namespace SWallet.CustomerService.Requests.Customer
{
    public class AddOrUpdateCustomerBankAccountRequest
    {
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
        public int BankId { get; set; }
    }
}
