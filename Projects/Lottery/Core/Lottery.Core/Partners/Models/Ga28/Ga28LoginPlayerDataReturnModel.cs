using Newtonsoft.Json;

namespace Lottery.Core.Partners.Models.Ga28
{
    public class Ga28LoginPlayerDataReturnModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("game_client-url")]
        public string GameClientUrl { get; set; }
        [JsonProperty("member")]
        public Ga28MemberInfoModel Member { get; set; }
    }
}
