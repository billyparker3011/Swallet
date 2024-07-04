using HnMicro.Modules.InMemory.Repositories;
using Lottery.Tools.AdjustOddsService.Models.Payouts;

namespace Lottery.Tools.AdjustOddsService.InMemory.Payouts
{
    public class PayoutByBetKindAndNumberInMemoryRepository : InMemoryRepository<string, PayoutByBetKindAndNumberModel>, IPayoutByBetKindAndNumberInMemoryRepository
    {
        public void RemoveByMatchId(long matchId)
        {
            var items = Items.Values.Where(f => f.MatchId == matchId).ToList();
            foreach (var item in items) InternalTryRemove(item);
        }

        protected override void InternalTryAddOrUpdate(PayoutByBetKindAndNumberModel item)
        {
            var id = item.ToString();
            if (!Items.TryGetValue(id, out PayoutByBetKindAndNumberModel val))
            {
                val = new PayoutByBetKindAndNumberModel
                {
                    BetKindId = item.BetKindId,
                    MatchId = item.MatchId,
                    Number = item.Number
                };
                Items[id] = val;
            }
            val.Payout += item.Payout;
        }

        protected override void InternalTryRemove(PayoutByBetKindAndNumberModel item)
        {
            var id = item.ToString();
            Items.TryRemove(id, out _);
        }
    }
}
