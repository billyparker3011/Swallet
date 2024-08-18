using HnMicro.Framework.Services;
using Lottery.Core.UnitOfWorks;

namespace Lottery.Core.Services.Wallet
{
    public class Ga28SingleWallet : AbstractPartnerSingleWallet
    {
        public Ga28SingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            return (0m, 0m);
        }
    }
}
