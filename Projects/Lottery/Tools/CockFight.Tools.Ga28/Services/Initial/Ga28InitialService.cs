using HnMicro.Framework.Exceptions;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Helpers.Converters.Bookie;
using Lottery.Core.Helpers.Converters.Partners;
using Lottery.Core.InMemory.Bookies;
using Lottery.Core.InMemory.Partner;
using Lottery.Core.Partners.Subscriber;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CockFight.Tools.Ga28.Services.Initial
{
    public class Ga28InitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public Ga28InitialService(IServiceProvider serviceProvider, IInMemoryUnitOfWork inMemoryUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InternalLoadDefaultData();
            await InternalSubscriber();
        }

        private async Task InternalLoadDefaultData()
        {
            var scope = _serviceProvider.CreateAsyncScope();
            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();

            await GetBookiesSettings(lotteryUow);
            await GetAllCockFightBetKinds(lotteryUow);
        }

        private async Task GetAllCockFightBetKinds(ILotteryUow lotteryUow)
        {
            var cockFightBetKindRepository = lotteryUow.GetRepository<ICockFightBetKindRepository>();
            var cockFightBetKinds = await cockFightBetKindRepository.FindQuery().ToListAsync();

            var cockFightBetKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICockFightBetKindInMemoryRepository>();
            cockFightBetKindInMemoryRepository.AddRange(cockFightBetKinds.Select(f => f.ToCockFightBetKindModel()).ToList());
        }

        private async Task GetBookiesSettings(ILotteryUow lotteryUow)
        {
            var bookieSettingRepository = lotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookies = await bookieSettingRepository.FindQuery().ToListAsync();

            var bookieSettingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBookieSettingInMemoryRepository>();
            bookieSettingInMemoryRepository.AddRange(bookies.Select(f => f.ToBookieSettingModel()).ToList());
        }

        private async Task InternalSubscriber()
        {
            var partnerChannelService = _serviceProvider.GetService<IPartnerSubscribeService>() ?? throw new HnMicroException();

            await partnerChannelService.Subscribe(Lottery.Core.Partners.Configs.PartnerChannelConfigs.Ga28Channel);
            await partnerChannelService.SubscribeBookieChannel(Lottery.Core.Partners.Configs.PartnerChannelConfigs.BookieChannel);
            await partnerChannelService.SubscribeGa28BetKindChannel(Lottery.Core.Partners.Configs.PartnerChannelConfigs.Ga28BetKindChannel);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
