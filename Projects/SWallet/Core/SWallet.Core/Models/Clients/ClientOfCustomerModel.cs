namespace SWallet.Core.Models.Clients
{
    public class ClientOfCustomerModel : ClientModel
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAffiliate { get; set; }
        public string Email { get; set; }
        public string Telegram { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
        public long AgentId { get; set; }
    }
}
