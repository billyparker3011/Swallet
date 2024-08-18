namespace Lottery.Core.Services.Wallet
{
    public interface IPartnerSingleWallet
    {
        Task<(decimal, decimal)> GetOutsAndWinlose(long playerId);
    }
}
