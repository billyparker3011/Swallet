using HnMicro.Framework.Services;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Services.Wallet
{
    internal class BtiSingleWallet : AbstractPartnerSingleWallet
    {
        private const int _offSetTime = 8;
        public BtiSingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            var btiTicketRepository = LotteryUow.GetRepository<IBtiTicketRepository>();
            var timeNow = ClockService.GetUtcNow().AddHours(_offSetTime);
            var startKickoffTime = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, 0, 0, 0);
            var endKickoffTime = startKickoffTime.AddDays(1);

            var ticket = await btiTicketRepository.FindQueryBy(c => c.PlayerId == playerId && c.Status != BtiTicketStatusHelper.Cancel && BtiTypeHelper.AmountType.Contains(c.Type) && c.TicketCreatedDate >= startKickoffTime && c.TicketCreatedDate <= endKickoffTime).ToListAsync();
            var reverseBetAmount = ticket.Where(c => BtiTypeHelper.Reverse == c.Type && c.Status == BtiTicketStatusHelper.Open).Select(c => c.BetAmount ?? 0m).Sum();
            var outs = ticket.Where(c => BtiTypeHelper.DebitReverse == c.Type && BtiTicketStatusHelper.Betted.Contains(c.Status)).Select(c => c.BetAmount ?? 0m).Sum() + reverseBetAmount;
            var winlose = ticket.Select(c => c.WinlossAmount ?? 0m).Sum();
            return (outs * 1000, winlose * 1000);
        }
    }
}
