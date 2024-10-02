namespace SWallet.Core.Models.Bank
{
    public class BankModel
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string Icon { get; set; }
        public bool DepositEnabled { get; set; }
        public bool WithdrawEnabled { get; set; }
    }
}
