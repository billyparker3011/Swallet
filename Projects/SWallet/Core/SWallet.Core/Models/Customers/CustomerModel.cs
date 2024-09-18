using SWallet.Core.Enums;

namespace SWallet.Core.Models.Customers
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
        public int RoleId { get; set; }
        public bool IsAffiliate { get; set; }
        public long AgentId { get; set; }
        public long MasterId { get; set; }
        public long SupermasterId { get; set; }
    }
}
