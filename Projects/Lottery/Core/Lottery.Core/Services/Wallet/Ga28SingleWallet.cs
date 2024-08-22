using HnMicro.Framework.Services;
using Lottery.Core.Helpers;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Services.Wallet
{
    public class Ga28SingleWallet : AbstractPartnerSingleWallet
    {
        private const int _offSetTime = 8;
        public Ga28SingleWallet(IClockService clockService, ILotteryUow lotteryUow) : base(clockService, lotteryUow)
        {
        }

        public override async Task<(decimal, decimal)> GetOutsAndWinlose(long playerId)
        {
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var ga28Now = ClockService.GetUtcNow().AddHours(_offSetTime);
            var startKickoffTime = new DateTime(ga28Now.Year, ga28Now.Month, ga28Now.Day, 0, 0, 0);
            var endKickoffTime = startKickoffTime.AddDays(1);
            var tickets = await cockFightTicketRepository.FindQueryBy(f => f.PlayerId == playerId && !f.ParentId.HasValue && f.TicketModifiedDate >= startKickoffTime && f.TicketModifiedDate < endKickoffTime && !CommonHelper.RefundRejectCockFightTicketState().Contains(f.Status)).ToListAsync();
            return (tickets.Select(x => x.TicketAmount ?? 0m * 1000).Sum(x => x), tickets.Select(x => x.WinlossAmount ?? 0m * 1000).Sum(x => x));
        }
    }
}
