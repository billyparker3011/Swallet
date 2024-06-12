using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Models.Player;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.Services.Match;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class PlayerCreditService : LotteryBaseService<PlayerCreditService>, IPlayerCreditService
    {
        private readonly IMatchService _matchService;
        private readonly IProcessTicketService _processTicketService;

        public PlayerCreditService(ILogger<PlayerCreditService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IMatchService matchService,
            IProcessTicketService processTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _matchService = matchService;
            _processTicketService = processTicketService;
        }

        public async Task<MyBalanceModel> GetMyBalance()
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(ClientContext.Player.PlayerId) ?? throw new NotFoundException();

            var outs = 0m;
            var runningMatch = await _matchService.GetRunningMatch();
            if (runningMatch != null)
            {
                var currentOuts = await _processTicketService.GetOuts(ClientContext.Player.PlayerId, runningMatch.MatchId);
                outs = currentOuts != null ? currentOuts.OutsByMatch : 0m;
            }

            return new MyBalanceModel
            {
                Username = ClientContext.Player.UserName,
                Credit = player.Credit,
                Outstanding = outs
            };
        }
    }
}
