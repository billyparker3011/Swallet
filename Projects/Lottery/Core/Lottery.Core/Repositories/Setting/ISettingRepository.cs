using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Setting
{
    public interface ISettingRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Setting, LotteryContext>
    {
    }
}
