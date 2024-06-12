using HnMicro.Modules.LoggerService.Services;

namespace HnMicro.LoggerService.Services
{
    public class InitialService : IHostedService
    {
        private readonly ISubscribeLoggerUsingRedisService _serverLoggerUsingPubSubRedisService;

        public InitialService(ISubscribeLoggerUsingRedisService serverLoggerUsingPubSubRedisService)
        {
            _serverLoggerUsingPubSubRedisService = serverLoggerUsingPubSubRedisService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _serverLoggerUsingPubSubRedisService.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
