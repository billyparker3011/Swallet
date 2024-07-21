using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Settings;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Lottery.Tools.AdjustOddsService.InMemory.Payouts;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;
using Lottery.Tools.AdjustOddsService.Services.PubSub;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Tools.AdjustOddsService.Services.InternalInitial
{
    public class InternalInitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IInternalSubscribeService _internalSubscribeService;

        public InternalInitialService(IServiceProvider serviceProvider, IInMemoryUnitOfWork inMemoryUnitOfWork, IInternalSubscribeService internalSubscribeService)
        {
            _serviceProvider = serviceProvider;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _internalSubscribeService = internalSubscribeService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadingData();
            _internalSubscribeService.Start();
        }

        private async Task LoadingData()
        {
            var scope = _serviceProvider.CreateAsyncScope();

            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            await GetAllSettings(lotteryUow);

            await GetAllRunningMixed(lotteryUow);

            var oddsAdjustmentService = scope.ServiceProvider.GetService<IOddsAdjustmentService>();
            oddsAdjustmentService.Enqueue(new ChangePayoutOfPairNumbersCommand
            {
                MatchId = 75L,
                PairNumbers = new Dictionary<int, List<string>>
                {
                    { 4, new List<string> { "00, 01" } }
                }
            });
        }

        private async Task GetAllRunningMixed(ILotteryUow lotteryUow)
        {
            var matchRepository = lotteryUow.GetRepository<IMatchRepository>();
            var runningMatch = await matchRepository.GetRunningMatch();
            if (runningMatch == null) return;

            var ticketRepository = lotteryUow.GetRepository<ITicketRepository>();
            var betKindIds = BetKind.FirstNorthern_Northern_LoXien.ToInt().BuildBetKinds();
            var runningState = CommonHelper.OutsTicketState();
            var tickets = await ticketRepository.FindQueryBy(f => f.MatchId == runningMatch.MatchId && betKindIds.Contains(f.BetKindId) && f.ParentId.HasValue && runningState.Contains(f.State)).ToListAsync();

            var payoutByMixedAndNumberInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPayoutByMixedAndNumberInMemoryRepository>();
            foreach (var item in tickets)
            {
                payoutByMixedAndNumberInMemoryRepository.Add(new Models.Payouts.PayoutByMixedAndNumberModel
                {
                    BetKindId = item.BetKindId,
                    MatchId = item.MatchId,
                    PairNumber = item.ChoosenNumbers,
                    Payout = (1 - item.SupermasterPt) * item.PlayerPayout
                });
            }
        }

        private async Task GetAllSettings(ILotteryUow lotteryUow)
        {
            var settingRepository = lotteryUow.GetRepository<ISettingRepository>();
            var allSettings = await settingRepository.FindQuery().ToListAsync();

            var settingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
            settingInMemoryRepository.AddRange(allSettings.Select(f => f.ToSettingModel()).ToList());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
