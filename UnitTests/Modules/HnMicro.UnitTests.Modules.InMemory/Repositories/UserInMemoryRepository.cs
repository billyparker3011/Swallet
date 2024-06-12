using HnMicro.Modules.InMemory.Repositories;
using HnMicro.UnitTests.Modules.InMemory.Models;

namespace HnMicro.UnitTests.Modules.InMemory.Repositories;

public class UserInMemoryRepository : InMemoryRepository<int, User>, IUserInMemoryRepository
{
    public User FindByUsername(string username)
    {
        return Items.Values.FirstOrDefault(f => f.Username == username);
    }

    protected override void InternalTryAddOrUpdate(User item)
    {
        Items[item.UserId] = item;
    }

    protected override void InternalTryRemove(User item)
    {
        Items.TryRemove(item.UserId, out _);
    }
}