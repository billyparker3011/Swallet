using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Region
{
    public class RegionInMemoryRepository : InMemoryRepository<Enums.Region, RegionModel>, IRegionInMemoryRepository
    {
        public RegionInMemoryRepository()
        {
            var items = typeof(Enums.Region).GetListEnumRegionInformation<Enums.Region>();
            foreach (var item in items)
                InternalTryAddOrUpdate(new RegionModel
                {
                    Id = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                    NoOfPrize = item.NoOfPrize
                });
        }

        protected override void InternalTryAddOrUpdate(RegionModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(RegionModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
