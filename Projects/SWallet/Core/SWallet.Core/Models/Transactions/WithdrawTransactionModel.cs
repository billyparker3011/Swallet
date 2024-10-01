namespace SWallet.Core.Models.Transactions
{
    public class WithdrawTransactionModel
    {
        //  From
        public string WithdrawBankName { get; set; }
        public string WithdrawNumberAccount { get; set; }
        public string WithdrawCardHolder { get; set; }
        //  To
        public string WithdrawToBankName { get; set; }
        public string WithdrawToNumberAccount { get; set; }
        public string WithdrawToCardHolder { get; set; }
    }
}
