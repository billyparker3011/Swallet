using Lottery.Player.OddsService.Services.PubSub;

namespace Lottery.Player.OddsService.Services.Initial
{
    public class InitialOddsService : IHostedService
    {
        private readonly ISubscribeMatchAndOddsService _subscribeMatchAndOddsService;

        public InitialOddsService(ISubscribeMatchAndOddsService subscribeMatchAndOddsService)
        {
            _subscribeMatchAndOddsService = subscribeMatchAndOddsService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _subscribeMatchAndOddsService.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
