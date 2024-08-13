using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoPlayerBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerBetSetting, LotteryContext>
    {
    }
}
