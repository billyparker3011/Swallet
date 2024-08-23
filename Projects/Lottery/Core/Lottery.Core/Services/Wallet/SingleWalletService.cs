using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Wallet
{
    public class SingleWalletService : LotteryBaseService<SingleWalletService>, ISingleWalletService
    {
        private readonly List<IPartnerSingleWallet> _bookies = new();

        public SingleWalletService(ILogger<SingleWalletService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            Initial();
        }

        private void Initial()
        {
            _bookies.Add(new M8XsSingleWallet(ClockService, LotteryUow));
            _bookies.Add(new Ga28SingleWallet(ClockService, LotteryUow));
            _bookies.Add(new AllbetSingleWallet(ClockService, LotteryUow));
        }

        public async Task<decimal> GetBalance(long playerId, decimal rate = 1m)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(playerId) ?? throw new NotFoundException();

            var totalOuts = 0m;
            var totalWinlose = 0m;
            foreach (var item in _bookies)
            {
                (var outs, var winlose) = await item.GetOutsAndWinlose(playerId);
                totalOuts += outs;
                totalWinlose += winlose;
            }
            var balance = player.Credit + totalWinlose - totalOuts;
            return balance / rate;
        }

        public async Task<decimal> GetBalanceForGa28(long playerId)
        {
            return await GetBalance(playerId, 1000m);
        }
    }
}
