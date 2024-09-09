using HnMicro.Framework.Services;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static Lottery.Core.Helpers.PartnerHelper;

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
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var timeNow = ClockService.GetUtcNow().AddHours(_offSetTime);
            var startKickoffTime = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, 0, 0, 0);
            var endKickoffTime = startKickoffTime.AddDays(1);
            var tickets = await casinoTicketRepository.FindQueryBy(f => f.PlayerId == playerId && f.CreatedAt >= startKickoffTime && f.CreatedAt < endKickoffTime && !f.IsCancel).Include(c => c.CasinoTicketBetDetails).ToListAsync();
            var ticketBetDetails = new List<CasinoTicketBetDetail>();
            tickets.ForEach(c => ticketBetDetails.AddRange(c.CasinoTicketBetDetails?.ToList())); 

            return (GetOuts(ticketBetDetails) * 1000 , tickets.Select(x => x.Amount * 1000).Sum(x => x) - GetOuts(ticketBetDetails) * 1000);
        }

        private decimal GetOuts(List<CasinoTicketBetDetail> items)
        {
            if (items == null || !items.Any()) return 0m;
            var betted = items.Where(c => CasinoBetStatus.BetCompleted.Contains(c.Status)).Select(x => x.GameRoundId)?.Distinct()?.ToList();
            return items.Where(c => !betted.Contains(c.GameRoundId)).Select(c => -1 * (c.BetAmount + c.Deposit)).Sum();
        }
    }
}
