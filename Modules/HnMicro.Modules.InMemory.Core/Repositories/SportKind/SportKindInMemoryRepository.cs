using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.Core.Models;
using HnMicro.Modules.InMemory.Repositories;

namespace HnMicro.Modules.InMemory.Core.Repositories.SportKind
{
    public class SportKindInMemoryRepository : InMemoryRepository<Framework.Enums.SportKind, SportKindModel>, ISportKindInMemoryRepository
    {
        public SportKindInMemoryRepository()
        {
            var items = typeof(Framework.Enums.SportKind).GetListEnumInformation<Framework.Enums.SportKind>();
            foreach (var item in items)
                InternalTryAddOrUpdate(new SportKindModel
                {
                    Id = item.Value,
                    Code = item.Code
                });
        }

        protected override void InternalTryAddOrUpdate(SportKindModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(SportKindModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
