using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Helpers.Converters.Settings;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Tools.AdjustOddsService.Services.InternalInitial
{
    public class InternalInitialService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public InternalInitialService(IServiceProvider serviceProvider, IInMemoryUnitOfWork inMemoryUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
