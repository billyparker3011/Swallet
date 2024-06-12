using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public interface IPlayerSessionRepository : IEntityFrameworkCoreRepository<long, Data.Entities.PlayerSession, LotteryContext>
    {
        Task<Data.Entities.PlayerSession> FindByPlayerId(long playerId);
    }
}
