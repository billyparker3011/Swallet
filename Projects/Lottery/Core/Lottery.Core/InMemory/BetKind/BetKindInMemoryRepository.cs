using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.InMemory.BetKind;

public class BetKindInMemoryRepository : InMemoryRepository<int, BetKindModel>, IBetKindInMemoryRepository
{
    protected override void InternalTryAddOrUpdate(BetKindModel item)
    {
        Items[item.Id] = item;
    }

    protected override void InternalTryRemove(BetKindModel item)
    {
        Items.TryRemove(item.Id, out _);
    }
}