namespace Lottery.Core.Models.Auth
{
    public class ResetSecurityCodeModel
    {
        public long TargetId { get; set; }
        public string SecurityCode { get; set; }
        public string ConfirmSecurityCode { get; set; }
        public bool IsSelfChange { get; set; }
    }
}
