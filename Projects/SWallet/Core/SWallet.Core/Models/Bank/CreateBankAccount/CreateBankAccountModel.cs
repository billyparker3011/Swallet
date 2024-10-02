namespace SWallet.Core.Models
{
    public class CreateBankAccountModel
    {
        public int BankId {  get; set; }
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
    }
}
