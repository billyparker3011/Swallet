namespace SWallet.Core.Models.Customers
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Telegram { get; set; }
        public string AffiliateCode { get; set; }
        public bool Accepted { get; set; }
        public int? DiscountId { get; set; }
    }
}
