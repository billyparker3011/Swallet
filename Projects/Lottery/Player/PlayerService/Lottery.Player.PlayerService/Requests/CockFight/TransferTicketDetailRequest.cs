using System.Text.Json.Serialization;

namespace Lottery.Player.PlayerService.Requests.CockFight
{
    public class TransferTicketDetailRequest
    {
        [JsonPropertyName("account_id")]
        public Guid AccountId { get; set; }

        [JsonPropertyName("ante_amount")]
        public string AnteAmount { get; set; }

        [JsonPropertyName("arena_code")]
        public string ArenaCode { get; set; }

        [JsonPropertyName("bet_amount")]
        public string BetAmount { get; set; }

        [JsonPropertyName("created_date_time")]
        public DateTime CreatedDateTime { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("fight_number")]
        public int FightNumber { get; set; }

        [JsonPropertyName("match_day_code")]
        public string MatchDayCode { get; set; }

        [JsonPropertyName("member_ref_id")]
        public string MemberRefId { get; set; }

        [JsonPropertyName("modified_date_time")]
        public DateTime? ModifiedDateTime { get; set; }

        [JsonPropertyName("odds")]
        public string Odds { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("selection")]
        public string Selection { get; set; }

        [JsonPropertyName("settled_date_time")]
        public DateTime? SettledDateTime { get; set; }

        [JsonPropertyName("sid")]
        public string SId { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("ticket_amount")]
        public string TicketAmount { get; set; }

        [JsonPropertyName("win_loss_amount")]
        public string WinlossAmount { get; set; }

        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }
    }
}
