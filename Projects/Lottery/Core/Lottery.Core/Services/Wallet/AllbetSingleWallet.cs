using HnMicro.Framework.Services;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Services.Wallet
{
    public class AllbetSingleWallet : AbstractPartnerSingleWallet
    {
        private const int _offSetTime = 8;
        public AllbetSingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            var timeNow = ClockService.GetUtcNow().AddHours(_offSetTime);
            var startKickoffTime = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, 0, 0, 0);
            var endKickoffTime = startKickoffTime.AddDays(1);
            var tickets = await casinoTicketRepository.FindQueryBy(f => f.PlayerId == playerId && f.CreatedAt >= startKickoffTime && f.CreatedAt < endKickoffTime && !f.IsCancel).ToListAsync();
            return (tickets.Select(x => x.Amount * 1000).Sum(x => x), tickets.Select(x => x.WinlossAmountTotal * 1000).Sum(x => x));
        }
    }
}
