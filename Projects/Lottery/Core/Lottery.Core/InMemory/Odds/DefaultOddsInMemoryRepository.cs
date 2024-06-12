using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Odds;

namespace Lottery.Core.InMemory.Odds
{
    public class DefaultOddsInMemoryRepository : InMemoryRepository<long, OddsModel>, IDefaultOddsInMemoryRepository
    {
        protected override void InternalTryAddOrUpdate(OddsModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(OddsModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
