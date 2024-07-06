using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Channel;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Payouts;
using Lottery.Core.Models.Prize;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Pubs
{
    public class PublishCommonService : IPublishCommonService
    {
        private readonly IRedisCacheService _redisCacheService;

        public PublishCommonService(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        public async Task PublishBetKind(List<BetKindModel> updatedBetKinds)
        {
            if (updatedBetKinds.Count == 0) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.BetKindConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(updatedBetKinds), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishChannel(List<ChannelModel> updatedChannels)
        {
            if (updatedChannels.Count == 0) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.ChannelConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(updatedChannels), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishCompanyPayouts(CompanyPayoutModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.CompanyPayoutChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishCompletedMatch(CompletedMatchModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.CompletedMatchChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishDefaultOdds(List<OddsModel> defaultOdds)
        {
            if (defaultOdds.Count == 0) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.DefaultOddsConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(defaultOdds), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishOddsValue(RateOfOddsValueModel rateOfOddsValue)
        {
            if (rateOfOddsValue == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.RateOfOddsValueConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(rateOfOddsValue), CachingConfigs.RedisConnectionForApp);
        }

        public void PublishOddsValueSingle(RateOfOddsValueModel rateOfOddsValue)
        {
            if (rateOfOddsValue == null) return;
            _redisCacheService.Publish(SubscribeCommonConfigs.RateOfOddsValueConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(rateOfOddsValue), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishPrize(List<PrizeModel> updatedPrizes)
        {
            if (updatedPrizes.Count == 0) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.PrizeConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(updatedPrizes), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishSetting(SettingModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.PublishSettingChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishStartLive(StartLiveEventModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.StartLiveConfigChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishUpdateLiveOdds(UpdateLiveOddsModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.UpdateLiveOddsChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }

        public async Task PublishUpdateMatch(UpdateMatchModel model)
        {
            if (model == null) return;
            await _redisCacheService.PublishAsync(SubscribeCommonConfigs.UpdateMatchChannel, Newtonsoft.Json.JsonConvert.SerializeObject(model), CachingConfigs.RedisConnectionForApp);
        }
    }
}
