using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums.Partner;
using Lottery.Data;
using Lottery.Data.Entities;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightPlayerBetSettingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.CockFightPlayerBetSetting, LotteryContext>
    {
        Task<CockFightPlayerBetSetting> FindBetSettingByPlayerId(long playerId);
    }
}
