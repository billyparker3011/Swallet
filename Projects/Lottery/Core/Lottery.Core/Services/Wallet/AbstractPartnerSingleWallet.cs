using HnMicro.Framework.Services;
using Lottery.Core.UnitOfWorks;

namespace Lottery.Core.Services.Wallet
{
    public abstract class AbstractPartnerSingleWallet : IPartnerSingleWallet
    {
        protected IClockService ClockService;
        protected ILotteryUow LotteryUow;

        protected AbstractPartnerSingleWallet(IClockService clockService, ILotteryUow lotteryUow)
        {
            ClockService = clockService;
            LotteryUow = lotteryUow;
        }

        public abstract Task<(decimal, decimal)> GetOutsAndWinlose(long playerId);
    }
}
