using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class UpdateMixedOddsMessage
    {
        [JsonProperty("m")]
        public long MatchId { get; set; }

        [JsonProperty("trs")]
        public List<UpdateMixedOddsDetailMessage> RateValues { get; set; }
    }
}
