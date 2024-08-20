using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.BetKind;

namespace Lottery.Core.InMemory.Partner
{
    public class CockFightBetKindInMemoryRepository : InMemoryRepository<int, CockFightBetKindModel>, ICockFightBetKindInMemoryRepository
    {
        public CockFightBetKindModel GetDefaultBetKind()
        {
            return Items.Values.FirstOrDefault();
        }

        protected override void InternalTryAddOrUpdate(CockFightBetKindModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(CockFightBetKindModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
