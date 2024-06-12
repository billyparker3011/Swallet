using HnMicro.Framework.Logger.Configs;
using HnMicro.Framework.Logger.Models;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.LoggerService.Providers;
using Microsoft.Extensions.Logging;

namespace HnMicro.Modules.LoggerService.Services
{
    public class SubscribeLoggerUsingRedisService : ISubscribeLoggerUsingRedisService
    {
        private readonly ILogger<SubscribeLoggerUsingRedisService> _logger;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IProviderFactory _providerFactory;

        public SubscribeLoggerUsingRedisService(ILogger<SubscribeLoggerUsingRedisService> logger, IRedisCacheService redisCacheService, IProviderFactory providerFactory)
        {
            _logger = logger;
            _redisCacheService = redisCacheService;
            _providerFactory = providerFactory;
        }

        public void Start()
        {
            _redisCacheService.Subscribe(LoggerConfig.LoggerServiceConfigChannel, (channel, message) =>
            {
                try
                {
                    _providerFactory.Enqueue(Newtonsoft.Json.JsonConvert.DeserializeObject<LogModel>(message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message, ex.StackTrace);
                }
            }, LoggerConfig.LoggerServiceConfigServerName);
        }
    }
}
