using HnMicro.Modules.InMemory.Repositories;
using Lottery.Tools.AdjustOddsService.Models.Payouts;

namespace Lottery.Tools.AdjustOddsService.InMemory.Payouts
{
    public interface IPayoutByBetKindAndNumberInMemoryRepository : IInMemoryRepository<string, PayoutByBetKindAndNumberModel>
    {
        void RemoveByMatchId(long matchId);
    }
}
