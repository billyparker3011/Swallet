using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Prize;

namespace Lottery.Core.InMemory.Prize
{
    public class PrizeInMemoryRepository : InMemoryRepository<int, PrizeModel>, IPrizeInMemoryRepository
    {
        public List<PrizeModel> FindByRegion(int regionId)
        {
            return Items.Values.Where(f => f.RegionId == regionId).ToList();
        }

        protected override void InternalTryAddOrUpdate(PrizeModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(PrizeModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
