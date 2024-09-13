namespace SWallet.Core.Dtos
{
    public class BankDto
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string Icon { get; set; }
        public bool DepositEnabled { get; set; }
        public bool WithdrawEnabled { get; set; }
    }
}
