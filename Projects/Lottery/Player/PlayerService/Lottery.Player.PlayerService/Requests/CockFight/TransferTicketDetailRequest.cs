using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Lottery.Player.PlayerService.Requests.CockFight
{
    public class TransferTicketDetailRequest
    {
        [JsonProperty("account_id"), JsonPropertyName("account_id")]
        public Guid AccountId { get; set; }

        [JsonProperty("ante_amount"), JsonPropertyName("ante_amount")]
        public string AnteAmount { get; set; }

        [JsonProperty("arena_code"), JsonPropertyName("arena_code")]
        public string ArenaCode { get; set; }

        [JsonProperty("bet_amount"), JsonPropertyName("bet_amount")]
        public string BetAmount { get; set; }

        [JsonProperty("created_date_time"), JsonPropertyName("created_date_time")]
        public DateTime CreatedDateTime { get; set; }

        [JsonProperty("currency_code"), JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("fight_number"), JsonPropertyName("fight_number")]
        public int FightNumber { get; set; }

        [JsonProperty("match_day_code"), JsonPropertyName("match_day_code")]
        public string MatchDayCode { get; set; }

        [JsonProperty("member_ref_id"), JsonPropertyName("member_ref_id")]
        public string MemberRefId { get; set; }

        [JsonProperty("modified_date_time"), JsonPropertyName("modified_date_time")]
        public DateTime? ModifiedDateTime { get; set; }

        public string Odds { get; set; }

        public string Result { get; set; }

        public string Selection { get; set; }

        [JsonProperty("settled_date_time"), JsonPropertyName("settled_date_time")]
        public DateTime? SettledDateTime { get; set; }

        public string SId { get; set; }
        public int Status { get; set; }

        [JsonProperty("ticket_amount"), JsonPropertyName("ticket_amount")]
        public string TicketAmount { get; set; }

        [JsonProperty("win_loss_amount"), JsonPropertyName("win_loss_amount")]
        public string WinlossAmount { get; set; }

        [JsonProperty("ip_address"), JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("user_agent"), JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }
    }
}
