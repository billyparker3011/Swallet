using Newtonsoft.Json;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28LoginPlayerDataReturnModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("game_client-url")]
        public string GameClientUrl { get; set; }
        public MemberInfo Member { get; set; }
    }

    public class MemberInfo
    {
        [JsonProperty("member_ref_id")]
        public string MemberRefId { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("freeze")]
        public bool Freeze { get; set; }
        [JsonProperty("main_limit_amount_per_fight")]
        public decimal? MainLimitAmountPerFight { get; set; }
        [JsonProperty("draw_limit_amount_per_fight")]
        public decimal? DrawLimitAmountPerFight { get; set; }
        [JsonProperty("limit_num_ticket_per_fight")]
        public decimal? LimitNumTicketPerFight { get; set; }
    }
}
