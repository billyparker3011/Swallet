namespace SWallet.ManagerService.Requests
{
    public class ChangePasswordRequest
    {
        public long CustomerId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
