using Lottery.Core.Services.Ticket;
using Microsoft.Extensions.Hosting;

namespace Lottery.Core.Services.Backgrounds
{
    public class ProcessPlayerWinloseBackgroundService : BackgroundService
    {
        private readonly IProcessPlayerWinloseService _processPlayerWinloseService;

        public ProcessPlayerWinloseBackgroundService(IProcessPlayerWinloseService processPlayerWinloseService)
        {
            _processPlayerWinloseService = processPlayerWinloseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processPlayerWinloseService.Start();
        }
    }
}
