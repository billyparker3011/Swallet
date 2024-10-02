namespace Lottery.Core.Models.Auth
{
    public class ResetPasswordModel
    {
        public long TargetId { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsSelfChange { get; set; }
    }
}
