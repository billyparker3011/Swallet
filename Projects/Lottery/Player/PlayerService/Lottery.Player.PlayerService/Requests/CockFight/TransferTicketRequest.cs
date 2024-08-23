using System.Text.Json.Serialization;

namespace Lottery.Player.PlayerService.Requests.CockFight
{
    public class TransferTicketRequest
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("member_ref_id")]
        public Guid MemberRefId { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("jackpot")]
        public string Jackpot { get; set; }

        [JsonPropertyName("ticket")]
        public TransferTicketDetailRequest Ticket { get; set; }
    }
}
