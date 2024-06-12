using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public interface IPlayerRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Player, LotteryContext>
    {
        Task<Data.Entities.Player> FindByUsernameAndPassword(string username, string password);
        Task<bool> CheckExistPlayer(string username);
    }
}
