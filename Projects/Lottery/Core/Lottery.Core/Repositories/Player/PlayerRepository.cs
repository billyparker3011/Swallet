using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Player
{
    public class PlayerRepository : EntityFrameworkCoreRepository<long, Data.Entities.Player, LotteryContext>, IPlayerRepository
    {
        public PlayerRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<bool> CheckExistPlayer(string username)
        {
            return await FindQueryBy(x => x.Username.ToLower() == username.ToLower()).AnyAsync();
        }
        public async Task<Data.Entities.Player> FindByUsernameAndPassword(string username, string password)
        {
            return await FindQueryBy(f => f.Username.Equals(username) && f.Password.Equals(password)).Include(f => f.PlayerSession).FirstOrDefaultAsync();
        }
    }
}
