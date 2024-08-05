using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Partners.Periodic;

namespace Lottery.Core.Partners.Subscriber
{
    public class PartnerSubscribeService : IPartnerSubscribeService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IPeriodicService _periodicService;

        public PartnerSubscribeService(IRedisCacheService redisCacheService, IPeriodicService periodicService)
        {
            _redisCacheService = redisCacheService;
            _periodicService = periodicService;
        }

        public async Task Subscribe(string channelName)
        {
            await _redisCacheService.SubscribeAsync(channelName, (channel, message) =>
            {
                _periodicService.Enqueue(message);
            }, CachingConfigs.RedisConnectionForApp);
        }
    }
}
