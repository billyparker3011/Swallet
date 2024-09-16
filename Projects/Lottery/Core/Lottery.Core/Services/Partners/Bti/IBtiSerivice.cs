using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiSerivice : IScopedDependency
    {
        string GenerateToken(long playerId, DateTime expiryTime);
        BtiOutTokenModel ValidateToken(string token);
    }
}
