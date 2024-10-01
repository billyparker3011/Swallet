namespace SWallet.ManagerService.Requests
{
    public class CreateBankAccountRequest
    {
        public int BankId { get; set; }
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
    }
}
