using HnMicro.Framework.Services;
using Lottery.Core.UnitOfWorks;

namespace Lottery.Core.Services.Wallet
{
    public class AllbetSingleWallet : AbstractPartnerSingleWallet
    {
        public AllbetSingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            return (0m, 0m);
        }
    }
}
