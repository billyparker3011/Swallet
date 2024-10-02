namespace SWallet.ManagerService.Requests.Discount
{
    public class DiscountSettingRequest
    {
        public DiscountDepositSettingRequest Deposit { get; set; }

        public DiscountWithdrawSettingRequest Withdraw { get; set; }
    }
}
