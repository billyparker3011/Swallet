using HnMicro.Modules.InMemory.Repositories;

namespace Lottery.Player.OddsService.InMemory.UserOnline
{
    public class UserOnlineInMemoryRepository : InMemoryRepository<string, Models.UserOnline>, IUserOnlineInMemoryRepository
    {
        public List<Models.UserOnline> FindAvailableUsers(int pongInSeconds = 30)
        {
            return Items.Values.Where(f => f.PongTime.AddSeconds(pongInSeconds) <= DateTime.UtcNow).ToList();
        }

        protected override void InternalTryAddOrUpdate(Models.UserOnline item)
        {
            Items[item.ConnectionId] = item;
        }

        protected override void InternalTryRemove(Models.UserOnline item)
        {
            Items.TryRemove(item.ConnectionId, out _);
        }
    }
}
