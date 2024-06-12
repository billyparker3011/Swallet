using HnMicro.Modules.InMemory.Repositories;

namespace Lottery.Player.OddsService.InMemory.UserOnline
{
    public interface IUserOnlineInMemoryRepository : IInMemoryRepository<string, Models.UserOnline>
    {
        List<Models.UserOnline> FindAvailableUsers(int pongInSeconds = 30);
    }
}
