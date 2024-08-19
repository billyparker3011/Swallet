using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Bookie;

namespace Lottery.Core.InMemory.Bookies
{
    public class BookieSettingInMemoryRepository : InMemoryRepository<int, BookieSettingModel>, IBookieSettingInMemoryRepository
    {
        protected override void InternalTryAddOrUpdate(BookieSettingModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(BookieSettingModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
