using SWallet.Core.Enums;

namespace SWallet.Core.Models.Customers
{
    public class MyCustomerProfileModel
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
    }
}
