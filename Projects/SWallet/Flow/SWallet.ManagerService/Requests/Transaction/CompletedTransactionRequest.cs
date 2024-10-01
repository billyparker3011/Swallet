namespace SWallet.ManagerService.Requests
{
    public class CompletedTransactionRequest
    {
        //  Begin: For Deposit
        public decimal Amount { get; set; }
        //  End: For Deposit

        //  Begin: For Withdraw
        public int BankId { get; set; }
        public int BankAccountId { get; set; }
        //  End: For Withdraw
    }
}
