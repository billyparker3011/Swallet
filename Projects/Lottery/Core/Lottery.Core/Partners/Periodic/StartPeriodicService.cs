using Microsoft.Extensions.Hosting;

namespace Lottery.Core.Partners.Periodic
{
    public class StartPeriodicService : BackgroundService
    {
        private readonly IPeriodicService _periodicService;

        public StartPeriodicService(IPeriodicService periodicService)
        {
            _periodicService = periodicService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _periodicService.Start();
        }
    }
}
