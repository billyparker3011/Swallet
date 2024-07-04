using HnMicro.Modules.InMemory.Repositories;
using Lottery.Tools.AdjustOddsService.Models.Payouts;

namespace Lottery.Tools.AdjustOddsService.InMemory.Payouts
{
    public interface IPayoutByBetKindAndNumberInMemoryRepository : IInMemoryRepository<string, PayoutByBetKindAndNumberModel>
    {
        PayoutByBetKindAndNumberModel FindByBetKindNumber(long matchId, int betKindId, int number);
        void RemoveByMatchId(long matchId);
    }
}
