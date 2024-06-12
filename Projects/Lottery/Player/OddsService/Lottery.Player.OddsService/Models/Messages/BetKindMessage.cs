using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class BetKindMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("b")]
        public decimal Buy { get; set; }
        [JsonProperty("tr")]
        public decimal TotalRate { get; set; }
    }
}
