using HnMicro.Modules.InMemory.Repositories;
using Lottery.Tools.AdjustOddsService.Models.Payouts;

namespace Lottery.Tools.AdjustOddsService.InMemory.Payouts
{
    public class PayoutByMixedAndNumberInMemoryRepository : InMemoryRepository<string, PayoutByMixedAndNumberModel>, IPayoutByMixedAndNumberInMemoryRepository
    {
        public PayoutByMixedAndNumberModel FindByBetKindNumber(long matchId, int betKindId, string pairNumber)
        {
            return Items.Values.FirstOrDefault(f => f.MatchId == matchId && f.BetKindId == betKindId && f.PairNumber == pairNumber);
        }

        public void RemoveByMatchId(long matchId)
        {
            var items = Items.Values.Where(f => f.MatchId == matchId).ToList();
            foreach (var item in items) InternalTryRemove(item);
        }

        protected override void InternalTryAddOrUpdate(PayoutByMixedAndNumberModel item)
        {
            var id = item.ToString();
            if (!Items.TryGetValue(id, out PayoutByMixedAndNumberModel val))
            {
                val = new PayoutByMixedAndNumberModel
                {
                    BetKindId = item.BetKindId,
                    MatchId = item.MatchId,
                    PairNumber = item.PairNumber
                };
                Items[id] = val;
            }
            val.Payout += item.Payout;
        }

        protected override void InternalTryRemove(PayoutByMixedAndNumberModel item)
        {
            var id = item.ToString();
            Items.TryRemove(id, out _);
        }
    }
}
