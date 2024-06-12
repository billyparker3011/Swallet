using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Category
{
    public class SubCategoryInMemoryRepository : InMemoryRepository<Enums.SubCategory, SubCategoryModel>, ISubCategoryInMemoryRepository
    {
        public SubCategoryInMemoryRepository()
        {
            var items = typeof(Enums.SubCategory).GetListEnumSubCategoryInformation<Enums.SubCategory>();
            foreach (var item in items)
                InternalTryAddOrUpdate(new SubCategoryModel
                {
                    Id = item.Value,
                    Name = item.Name,
                    OrderBy = item.OrderBy,
                    Category = item.Category,
                    BetKind = item.BetKind,
                    SubBetKinds = item.SubBetKinds
                });
        }

        protected override void InternalTryAddOrUpdate(SubCategoryModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(SubCategoryModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
