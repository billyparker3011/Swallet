using HnMicro.Framework.Services;
using Lottery.Core.Helpers;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Services.Wallet
{
    public class M8XsSingleWallet : AbstractPartnerSingleWallet
    {
        private const int _offSetTime = 7;

        public M8XsSingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
            var xsNow = ClockService.GetUtcNow().AddHours(_offSetTime);
            var startKickoffTime = new DateTime(xsNow.Year, xsNow.Month, xsNow.Day, 0, 0, 0);
            var endKickoffTime = startKickoffTime.AddDays(1);
            var tickets = await ticketRepository.FindQueryBy(f => f.PlayerId == playerId && !f.ParentId.HasValue && f.KickOffTime >= startKickoffTime && f.KickOffTime < endKickoffTime && !CommonHelper.RefundRejectTicketState().Contains(f.State)).ToListAsync();
            return (tickets.Sum(f => f.State), tickets.Sum(f => f.PlayerWinLoss));
        }
    }
}
