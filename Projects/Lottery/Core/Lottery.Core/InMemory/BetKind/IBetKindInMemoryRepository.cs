using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.InMemory.BetKind;

public interface IBetKindInMemoryRepository : IInMemoryRepository<int, BetKindModel>
{
    int GetLiveBetKindByRegion(int regionId);
}
