using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class StartLiveMessage
    {
        [JsonProperty("m")]
        public long MatchId { get; set; }

        [JsonProperty("r")]
        public int RegionId { get; set; }
    }
}
