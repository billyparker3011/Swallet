using HnMicro.Modules.InMemory.Repositories;
using Lottery.Tools.AdjustOddsService.Models.Payouts;

namespace Lottery.Tools.AdjustOddsService.InMemory.Payouts
{
    public interface IPayoutByMixedAndNumberInMemoryRepository : IInMemoryRepository<string, PayoutByMixedAndNumberModel>
    {
        PayoutByMixedAndNumberModel FindByBetKindNumber(long matchId, int betKindId, string pairNumber);
        void RemoveByMatchId(long matchId);
    }
}
