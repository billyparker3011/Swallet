using SWallet.Core.Enums;

namespace SWallet.Core.Models
{
    public class CustomerModel
    {
        public long CustomerId { get; set; }
        public string Username { get; set; }
        public string UsernameUpper { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telegram { get; set; }
        public CustomerState State { get; set; }
        public string Phone { get; set; }
        public bool Lock { get; set; }
        public bool DepositAllowed { get; set; }
        public bool WithdrawAllowed { get; set; }
        public bool DiscountAllowed { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public bool IsAffiliate { get; set; }
        public string AffiliateUsername { get; set; }
        public long AgentId { get; set; }
        public long MasterId { get; set; }
        public long SupermasterId { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ChangedPasswordAt { get; set; }
    }
}
