using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Helpers.Converters.Settings;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.UnitOfWorks;
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
