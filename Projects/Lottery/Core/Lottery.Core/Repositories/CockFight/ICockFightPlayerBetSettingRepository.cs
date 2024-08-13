using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums.Partner;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightPlayerBetSettingRepository : IEntityFrameworkCoreRepository<long, CockFightPlayerBetSetting, LotteryContext>
    {
        Task<CockFightPlayerBetSetting> FindBetSettingByPlayerId(long playerId);
    }
}
