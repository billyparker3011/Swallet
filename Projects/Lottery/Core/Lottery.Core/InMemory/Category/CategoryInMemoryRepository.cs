using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.Category
{
    public class CategoryInMemoryRepository : InMemoryRepository<Enums.Category, CategoryModel>, ICategoryInMemoryRepository
    {
        public CategoryInMemoryRepository()
        {
            var items = typeof(Enums.Category).GetListEnumCategoryInformation<Enums.Category>();
            foreach (var item in items)
                InternalTryAddOrUpdate(new CategoryModel
                {
                    Id = item.Value,
                    Code = item.Code,
                    Name = item.Name,
                    Region = item.Region,
                    OrderBy = item.OrderBy
                });
        }

        protected override void InternalTryAddOrUpdate(CategoryModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(CategoryModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
