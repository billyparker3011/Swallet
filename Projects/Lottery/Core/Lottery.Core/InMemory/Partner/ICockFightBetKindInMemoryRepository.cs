using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.InMemory.Partner
{
    public interface ICockFightBetKindInMemoryRepository : IInMemoryRepository<int, CockFightBetKindModel>
    {
        CockFightBetKindModel GetDefaultBetKind();
    }
}
