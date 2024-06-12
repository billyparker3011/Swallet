using HnMicro.Core.Scopes;
using Lottery.Core.Models.Authentication;

namespace Lottery.Core.Services.Authentication
{
    public interface ISessionService : IScopedDependency
    {
        Task CreateSession(SessionModel session);
        Task RemoveSession(int roleId, long targetId);
        Task<int> RecheckIn(bool isAgent = true);
    }
}
