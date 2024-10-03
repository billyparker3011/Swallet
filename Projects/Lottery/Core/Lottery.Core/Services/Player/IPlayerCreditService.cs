using HnMicro.Core.Scopes;
using Lottery.Core.Models.Player;

namespace Lottery.Core.Services.Player
{
    public interface IPlayerCreditService : IScopedDependency
    {
        Task<MyBalanceModel> GetMyBalance();
    }
}
