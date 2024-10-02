using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public interface IBtiPlayerBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiPlayerBetSetting, LotteryContext>
    {

    }
}
