using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Wallet
{
    public interface ISingleWalletService : IScopedDependency
    {
        Task<decimal> GetBalance(long playerId, decimal rate = 1m);
        Task<decimal> GetBalanceForGa28(long playerId);
        Task<decimal> GetBalanceForCasinoAllBet(long playerId);
        Task<decimal> GetBalanceForBti(long playerId);
    }
}
