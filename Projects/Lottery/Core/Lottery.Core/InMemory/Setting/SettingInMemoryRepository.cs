using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.InMemory.Setting
{
    public class SettingInMemoryRepository : InMemoryRepository<int, SettingModel>, ISettingInMemoryRepository
    {
        protected override void InternalTryAddOrUpdate(SettingModel item)
        {
            Items[item.Id] = item;
        }

        protected override void InternalTryRemove(SettingModel item)
        {
            Items.TryRemove(item.Id, out _);
        }
    }
}
