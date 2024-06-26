using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Player.OddsService.Hubs;

namespace Lottery.Player.OddsService.Services.PubSub
{
    public class SubscribeMatchAndOddsService : ISubscribeMatchAndOddsService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IOddsHubHandler _oddsHubHandler;

        public SubscribeMatchAndOddsService(IRedisCacheService redisCacheService, IOddsHubHandler oddsHubHandler)
        {
            _redisCacheService = redisCacheService;
            _oddsHubHandler = oddsHubHandler;
        }

        public void Start()
        {
            SubscribeUpdateLiveOdds();
            SubscribeRateOfOddsValue();
            SubscribeUpdateMatch();
            SubscribeStartLive();
        }

        private void SubscribeUpdateLiveOdds()
        {
            _redisCacheService.SubscribeAsync(SubscribeCommonConfigs.UpdateLiveOddsChannel, (channel, message) =>
            {
                ProcessUpdateLiveOdds(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeUpdateMatch()
        {
            _redisCacheService.SubscribeAsync(SubscribeCommonConfigs.UpdateMatchChannel, (channel, message) =>
            {
                ProcessUpdateMatch(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeStartLive()
        {
            _redisCacheService.SubscribeAsync(SubscribeCommonConfigs.StartLiveConfigChannel, (channel, message) =>
            {
                ProcessStartLive(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeRateOfOddsValue()
        {
            _redisCacheService.SubscribeAsync(SubscribeCommonConfigs.RateOfOddsValueConfigChannel, (channel, message) =>
            {
                ProcessRateOfOddsValueConfig(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void ProcessUpdateLiveOdds(string message)
        {
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateLiveOddsModel>(message);
            if (model == null) return;
            Task.Run(async () => await _oddsHubHandler.UpdateLiveOdds(model));
        }

        private void ProcessUpdateMatch(string message)
        {
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateMatchModel>(message);
            if (model == null) return;
            Task.Run(async () => await _oddsHubHandler.UpdateMatch(model));
        }

        private void ProcessStartLive(string message)
        {
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<StartLiveEventModel>(message);
            if (model == null) return;
            Task.Run(async () => await _oddsHubHandler.StartLive(model));
        }

        private void ProcessRateOfOddsValueConfig(string message)
        {
            var rateOfOddsValue = Newtonsoft.Json.JsonConvert.DeserializeObject<RateOfOddsValueModel>(message);
            if (rateOfOddsValue == null) return;
            Task.Run(async () => await _oddsHubHandler.UpdateOdds(rateOfOddsValue));
        }
    }
}
