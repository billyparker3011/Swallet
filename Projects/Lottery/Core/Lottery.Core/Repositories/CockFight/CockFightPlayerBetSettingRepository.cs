using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightPlayerBetSettingRepository : EntityFrameworkCoreRepository<long, CockFightPlayerBetSetting, LotteryContext>, ICockFightPlayerBetSettingRepository
    {
        public CockFightPlayerBetSettingRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<CockFightPlayerBetSetting> FindBetSettingByPlayerId(long playerId)
        {
            return await FindQuery().FirstOrDefaultAsync(f => f.PlayerId == playerId);
        }
    }
}
