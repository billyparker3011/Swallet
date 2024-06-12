using HnMicro.Modules.InMemory.Repositories;
using HnMicro.UnitTests.Modules.InMemory.Models;

namespace HnMicro.UnitTests.Modules.InMemory.Repositories;

public interface IUserInMemoryRepository : IInMemoryRepository<int, User>
{
    User FindByUsername(string username);
}
