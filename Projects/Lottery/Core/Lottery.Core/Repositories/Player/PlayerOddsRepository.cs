using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Player
{
    public class PlayerOddsRepository : EntityFrameworkCoreRepository<long, Data.Entities.PlayerOdd, LotteryContext>, IPlayerOddsRepository
    {
        public PlayerOddsRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<Data.Entities.PlayerOdd> GetBetSettingByPlayerAndBetKind(long playerId, int betKindId)
        {
            return await FindQueryBy(f => f.PlayerId == playerId && f.BetKindId == betKindId).FirstOrDefaultAsync();
        }
    }
}
