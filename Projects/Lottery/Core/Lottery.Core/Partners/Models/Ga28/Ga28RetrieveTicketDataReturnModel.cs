using Newtonsoft.Json;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28RetrieveTicketDataReturnModel
    {
        [JsonProperty("sid")]
        public string Sid { get; set; }
        [JsonProperty("created_date_time")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("modified_date_time")]
        public DateTime? ModifiedDateTime { get; set; }
        [JsonProperty("arena_code")]
        public string ArenaCode { get; set; }
        [JsonProperty("match_day_code")]
        public string MatchDayCode { get; set; }
        [JsonProperty("fight_number")]
        public int FightNumber { get; set; }
        [JsonProperty("selection")]
        public string Selection { get; set; }
        [JsonProperty("odds")]
        public string Odds { get; set; }
        [JsonProperty("bet_amount")]
        public string BetAmount { get; set; }
        [JsonProperty("valid_stake")]
        public string ValidStake { get; set; }
        [JsonProperty("ticket_amount")]
        public string TicketAmount { get; set; }
        [JsonProperty("ante_amount")]
        public string AnteAmount { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("member_ref_id")]
        public string MemberRefId { get; set; }
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonProperty("win_loss_amount")]
        public string WinLossAmount { get; set; }
        [JsonProperty("odds_type")]
        public string OddsType { get; set; }
        [JsonProperty("settled_date_time")]
        public DateTime? SettledDateTime { get; set; }
        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }
        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
    }
}
