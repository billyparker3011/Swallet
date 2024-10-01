namespace SWallet.ManagerService.Requests.Discount
{
    public class DiscountWithdrawSettingRequest
    {
        public decimal MinDepositAmount { get; set; }   //  Tong tien nap toi thieu
        public int NoOfTickets { get; set; }    //  So ve cuoc
    }
}
