using HnMicro.Core.Scopes;
using HnMicro.Framework.Models;
using System.Security.Claims;

namespace HnMicro.Framework.Services
{
    public interface IJwtTokenService : IScopedDependency
    {
        JwtToken BuildToken(List<Claim> claims);
    }
}
