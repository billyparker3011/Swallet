using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class UpdateLiveOddsMessage
    {
        [JsonProperty("n")]
        public int NoOfRemainingNumbers { get; set; }
        [JsonProperty("o")]
        public List<OddsByNumberMessage> OddsValue { get; set; }
    }
}
