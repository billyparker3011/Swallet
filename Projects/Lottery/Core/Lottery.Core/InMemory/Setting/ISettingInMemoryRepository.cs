using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.InMemory.Setting
{
    public interface ISettingInMemoryRepository : IInMemoryRepository<int, SettingModel>
    {
        SettingModel FindByKey(string keySetting);
    }
}
