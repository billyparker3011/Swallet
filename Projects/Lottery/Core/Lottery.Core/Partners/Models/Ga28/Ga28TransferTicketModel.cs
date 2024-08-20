using Newtonsoft.Json;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28TransferTicketModel
    {
        public Guid Id { get; set; }
        public string Amount { get; set; }
        [JsonProperty("member_ref_id")]
        public string MemberRefId { get; set; }
        public int Type { get; set; }
        public string Jackpot { get; set; }
        public Ga28RetrieveTicketDataReturnModel Ticket { get; set; }
    }
}
