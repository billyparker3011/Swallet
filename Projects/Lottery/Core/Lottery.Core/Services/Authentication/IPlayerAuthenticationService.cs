using HnMicro.Core.Scopes;
using HnMicro.Framework.Models;
using Lottery.Core.Models.Auth;

namespace Lottery.Core.Services.Authentication
{
    public interface IPlayerAuthenticationService : IScopedDependency
    {
        Task<JwtToken> Auth(AuthModel model);
    }
}
