namespace SWallet.Core.Models.Payment
{
    public class BankAccountForModel
    {
        public int BankAccountId { get; set; }
        public int BankId { get; set; }
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
    }
}
