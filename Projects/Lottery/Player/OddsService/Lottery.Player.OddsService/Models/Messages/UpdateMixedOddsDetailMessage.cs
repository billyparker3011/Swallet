using Newtonsoft.Json;

namespace Lottery.Player.OddsService.Models.Messages
{
    public class UpdateMixedOddsDetailMessage
    {
        [JsonProperty("bk")]
        public int BetKindId { get; set; }

        [JsonProperty("tr")]
        public decimal TotalRate { get; set; }
    }
}
