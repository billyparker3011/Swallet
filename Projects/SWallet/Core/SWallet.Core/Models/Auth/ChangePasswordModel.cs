namespace SWallet.Core.Models.Auth
{
    public class ChangePasswordModel
    {
        public long CustomerId { get; set; } = 0L;
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
