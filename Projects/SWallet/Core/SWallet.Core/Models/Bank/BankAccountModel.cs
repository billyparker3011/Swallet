namespace SWallet.Core.Models.Bank
{
    public class BankAccountModel
    {
        public int BankAccountId { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
    }
}
