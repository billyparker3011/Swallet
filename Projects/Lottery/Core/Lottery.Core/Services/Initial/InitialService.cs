using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Helpers.Converters.BetKinds;
using Lottery.Core.Helpers.Converters.Bookie;
using Lottery.Core.Helpers.Converters.Channels;
using Lottery.Core.Helpers.Converters.Odds;
using Lottery.Core.Helpers.Converters.Partners;
using Lottery.Core.Helpers.Converters.Prizes;
using Lottery.Core.Helpers.Converters.Settings;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Bookies;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Odds;
using Lottery.Core.InMemory.Partner;
using Lottery.Core.InMemory.Prize;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.Channel;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Prize;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.Services.Subscribe;
using Lottery.Core.Services.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lottery.Core.Services.Initial
{
    public class InitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly ISubscribeCommonService _subscribeCommonService;
        private readonly IScanTicketService _scanTicketService;

        public InitialService(IServiceProvider serviceProvider, IInMemoryUnitOfWork inMemoryUnitOfWork,
            ISubscribeCommonService subscribeCommonService,
            IScanTicketService scanTicketService)
        {
            _serviceProvider = serviceProvider;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _subscribeCommonService = subscribeCommonService;
            _scanTicketService = scanTicketService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateAsyncScope();

            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();
            await GetAllBetKinds(lotteryUow);
            await GetAllChannels(lotteryUow);
            await GetAllPrize(lotteryUow);
            await GetAllDefaultOdds(lotteryUow);
            await GetAllSettings(lotteryUow);

            await GetBookiesSettings(lotteryUow);
            await GetAllCockFightBetKinds(lotteryUow);

            _subscribeCommonService.Start();
            _scanTicketService.Start();
        }

        private async Task GetAllCockFightBetKinds(ILotteryUow lotteryUow)
        {
            var cockFightBetKindRepository = lotteryUow.GetRepository<ICockFightBetKindRepository>();
            var cockFightBetKinds = await cockFightBetKindRepository.FindQuery().ToListAsync();

            var cockFightBetKindInMemoryRepository = lotteryUow.GetRepository<ICockFightBetKindInMemoryRepository>();
            cockFightBetKindInMemoryRepository.AddRange(cockFightBetKinds.Select(f => f.ToCockFightBetKindModel()).ToList());
        }

        private async Task GetBookiesSettings(ILotteryUow lotteryUow)
        {
            var bookieSettingRepository = lotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookies = await bookieSettingRepository.FindQuery().ToListAsync();

            var bookieSettingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBookieSettingInMemoryRepository>();
            bookieSettingInMemoryRepository.AddRange(bookies.Select(f => f.ToBookieSettingModel()).ToList());
        }

        private async Task GetAllSettings(ILotteryUow lotteryUow)
        {
            var settingRepository = lotteryUow.GetRepository<ISettingRepository>();
            var allSettings = await settingRepository.FindQuery().ToListAsync();

            var settingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
            settingInMemoryRepository.AddRange(allSettings.Select(f => f.ToSettingModel()).ToList());
        }

        private async Task GetAllDefaultOdds(ILotteryUow lotteryUow)
        {
            var defaultOddsRepository = lotteryUow.GetRepository<IAgentOddsRepository>();
            var defaultOdds = await defaultOddsRepository.FindDefaultOdds();

            var defaultOddsInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IDefaultOddsInMemoryRepository>();
            defaultOddsInMemoryRepository.AddRange(defaultOdds.Select(f => f.ToOddsModel()).ToList());
        }

        private async Task GetAllPrize(ILotteryUow lotteryUow)
        {
            var prizeRepository = lotteryUow.GetRepository<IPrizeRepository>();
            var prizes = await prizeRepository.FindByAsync(f => true);

            var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
            prizeInMemoryRepository.AddRange(prizes.Select(f => f.ToPrizeModel()).ToList());
        }

        private async Task GetAllChannels(ILotteryUow lotteryUow)
        {
            var channelRepository = lotteryUow.GetRepository<IChannelRepository>();
            var channels = await channelRepository.FindQuery().ToListAsync();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            channelInMemoryRepository.AddRange(channels.Select(f => f.ToChannelModel()).ToList());
        }

        private async Task GetAllBetKinds(ILotteryUow lotteryUow)
        {
            var betKindRepository = lotteryUow.GetRepository<IBetKindRepository>();
            var betKinds = await betKindRepository.FindQuery().ToListAsync();

            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            betKindInMemoryRepository.AddRange(betKinds.Select(f => f.ToBetKindModel()).ToList());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
