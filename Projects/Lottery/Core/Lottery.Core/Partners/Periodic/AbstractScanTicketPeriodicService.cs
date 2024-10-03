using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners.Periodic
{
    public abstract class AbstractScanTicketPeriodicService<T> : IScanTicketPeriodicService
    {
        protected ILogger<T> Logger;
        protected IServiceProvider ServiceProvider;
        private volatile bool _initial;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

        protected AbstractScanTicketPeriodicService(ILogger<T> logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            ServiceProvider = serviceProvider;

            Init();
        }

        private void Init()
        {
            if (_initial) return;

            try
            {
                _cts = new CancellationTokenSource();
                _timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            }
            finally
            {
                _initial = true;
            }
        }

        public async Task Start()
        {
            Logger.LogInformation("Scan Tickets - Started.");

            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                await InternalScanTickets();
                await Task.Delay(1000);
            }
        }

        protected virtual async Task InternalScanTickets()
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var partnerService = scope.ServiceProvider.GetService<IPartnerService>();
                await partnerService.ScanTickets(new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0} - {1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
