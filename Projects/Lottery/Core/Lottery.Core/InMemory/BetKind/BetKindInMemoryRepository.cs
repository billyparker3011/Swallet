using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.InMemory.BetKind;

public class BetKindInMemoryRepository : InMemoryRepository<int, BetKindModel>, IBetKindInMemoryRepository
{
    public int GetLiveBetKindByRegion(int regionId)
    {
        var item = Items.Values.FirstOrDefault(f => f.RegionId == regionId && f.IsLive);
        return item != null ? item.Id : 0;
    }

    protected override void InternalTryAddOrUpdate(BetKindModel item)
    {
        Items[item.Id] = item;
    }

    protected override void InternalTryRemove(BetKindModel item)
    {
        Items.TryRemove(item.Id, out _);
    }
}