using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public interface IPlayerOddRepository : IEntityFrameworkCoreRepository<long, Data.Entities.PlayerOdd, LotteryContext>
    {
        Task<Data.Entities.PlayerOdd> GetBetSettingByPlayerAndBetKind(long playerId, int betKindId);
    }
}
