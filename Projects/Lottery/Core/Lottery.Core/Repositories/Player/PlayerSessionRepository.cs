using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Player
{
    public class PlayerSessionRepository : EntityFrameworkCoreRepository<long, PlayerSession, LotteryContext>, IPlayerSessionRepository
    {
        public PlayerSessionRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<PlayerSession> FindByPlayerId(long playerId)
        {
            return await FindQueryBy(f => f.PlayerId == playerId).FirstOrDefaultAsync();
        }
    }
}
