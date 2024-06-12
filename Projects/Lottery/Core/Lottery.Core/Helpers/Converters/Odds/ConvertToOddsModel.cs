using Lottery.Core.Models.Odds;
using Lottery.Data.Entities;

namespace Lottery.Core.Helpers.Converters.Odds
{
    public static class ConvertToOddsModel
    {
        public static OddsModel ToOddsModel(this AgentOdd agentOdds)
        {
            return new OddsModel
            {
                Id = agentOdds.Id,
                BetKindId = agentOdds.BetKindId,
                Buy = agentOdds.Buy,
                MinBuy = agentOdds.MinBuy,
                MaxBuy = agentOdds.MaxBuy,
                MinBet = agentOdds.MinBet,
                MaxBet = agentOdds.MaxBet,
                MaxPerNumber = agentOdds.MaxPerNumber
            };
        }
    }
}
