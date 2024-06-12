using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class OddsByNumberMessage
    {
        [JsonProperty("n")]
        public int Number { get; set; }
        [JsonProperty("bks")]
        public List<BetKindMessage> BetKinds { get; set; }
    }
}
