namespace SWallet.Core.Models.Transactions
{
    public class DepositTransactionModel
    {
        //  From
        public string DepositBankName { get; set; }
        public string DepositNumberAccount { get; set; }
        public string DepositCardHolder { get; set; }
        public string DepositContent { get; set; }
        //  To
        public string DepositToBankName { get; set; }
        public string DepositToNumberAccount { get; set; }
        public string DepositToCardHolder { get; set; }
    }
}
