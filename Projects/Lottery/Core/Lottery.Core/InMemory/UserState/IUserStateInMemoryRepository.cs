using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Enums;

namespace Lottery.Core.InMemory.UserState
{
    public interface IUserStateInMemoryRepository : IInMemoryRepository<Enums.UserState, UserStateModel>
    {
    }
}
