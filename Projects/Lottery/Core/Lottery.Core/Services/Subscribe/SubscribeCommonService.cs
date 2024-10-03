using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Odds;
using Lottery.Core.InMemory.Prize;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Channel;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Prize;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Subscribe
{
    public class SubscribeCommonService : ISubscribeCommonService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public SubscribeCommonService(IRedisCacheService redisCacheService, IInMemoryUnitOfWork inMemoryUnitOfWork)
        {
            _redisCacheService = redisCacheService;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        public void Start()
        {
            SubscribeBetKinds();
            SubscribeChannels();
            SubscribePrizes();
            SubscribeSettings();
            SubscribeDefaultOddsValue();
        }

        private void SubscribeSettings()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.PublishSettingChannel, (channel, message) =>
            {
                ProcessSetting(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribePrizes()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.PrizeConfigChannel, (channel, message) =>
            {
                ProcessPrize(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeDefaultOddsValue()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.DefaultOddsConfigChannel, (channel, message) =>
            {
                ProcessDefaultOddsConfig(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeChannels()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.ChannelConfigChannel, (channel, message) =>
            {
                ProcessChannel(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeBetKinds()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.BetKindConfigChannel, (channel, message) =>
            {
                ProcessBetKind(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void ProcessDefaultOddsConfig(string message)
        {
            var listDefaultOdds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OddsModel>>(message);
            var inmemoryDefaultOddsRepository = _inMemoryUnitOfWork.GetRepository<IDefaultOddsInMemoryRepository>();
            inmemoryDefaultOddsRepository.AddRange(listDefaultOdds);
        }

        private void ProcessChannel(string message)
        {
            var listChannel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChannelModel>>(message);
            var inmemoryChannelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            inmemoryChannelRepository.AddRange(listChannel);
        }

        private void ProcessBetKind(string message)
        {
            var listBetKind = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BetKindModel>>(message);
            var inmemoryBetKindRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            inmemoryBetKindRepository.AddRange(listBetKind);
        }

        private void ProcessPrize(string message)
        {
            var listPrize = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeModel>>(message);
            var inmemoryPrizeRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
            inmemoryPrizeRepository.AddRange(listPrize);
        }

        private void ProcessSetting(string message)
        {
            var setting = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingModel>(message);
            if (setting == null || setting.Id == 0) return;
            var inmemorySettingRepository = _inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
            inmemorySettingRepository.Update(setting);
        }
    }
}
