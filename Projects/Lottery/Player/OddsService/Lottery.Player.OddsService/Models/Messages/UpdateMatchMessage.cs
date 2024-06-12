using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class UpdateMatchMessage
    {
        [JsonProperty("m")]
        public long MatchId { get; set; }
    }
}
