using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Setting
{
    public interface ISettingRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Setting, LotteryContext>
    {
        Task<Data.Entities.Setting> FindByKey(string key);
    }
}
