using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class UpdateOddsMessage
    {
        [JsonProperty("m")]
        public long MatchId { get; set; }

        [JsonProperty("bk")]
        public int BetKindId { get; set; }

        [JsonProperty("n")]
        public int Number { get; set; }

        [JsonProperty("tr")]
        public decimal TotalRate { get; set; }
    }
}
