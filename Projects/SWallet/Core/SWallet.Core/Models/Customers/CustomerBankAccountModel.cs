namespace SWallet.Core.Models
{
    public class CustomerBankAccountModel
    {
        public long Id { get; set; }
        public int BankId { get; set; }
        public string BankIcon { get; set; }
        public string BankName { get; set; }
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
    }
}
