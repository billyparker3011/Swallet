using Microsoft.Extensions.Hosting;

namespace Lottery.Core.Partners.Periodic
{
    public class StartScanTicketPeriodicService : BackgroundService
    {
        private readonly IScanTicketPeriodicService _scanTicketPeriodicService;

        public StartScanTicketPeriodicService(IScanTicketPeriodicService scanTicketPeriodicService)
        {
            _scanTicketPeriodicService = scanTicketPeriodicService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _scanTicketPeriodicService.Start();
        }
    }
}
