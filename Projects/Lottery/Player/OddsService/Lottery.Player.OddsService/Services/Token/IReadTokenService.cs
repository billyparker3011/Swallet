using HnMicro.Core.Scopes;
using Lottery.Core.Models.Client;

namespace Lottery.Player.OddsService.Services.Token
{
    public interface IReadTokenService : ISingletonDependency
    {
        ClientPlayerModel ReadToken(string accessToken);
    }
}
