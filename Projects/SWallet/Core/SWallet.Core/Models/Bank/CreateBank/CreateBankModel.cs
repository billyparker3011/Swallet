namespace SWallet.Core.Models
{
    public class CreateBankModel
    {
        public string Name { get; set; }
        public string Icon {  get; set; }
        public bool DepositEnabled { get; set; }
        public bool WithdrawEnabled { get; set; }
    }
}
