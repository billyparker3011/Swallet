using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.InMemory.Bookies;
using Lottery.Core.InMemory.Partner;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Bookie;
using Lottery.Core.Partners.Periodic;

namespace Lottery.Core.Partners.Subscriber
{
    public class PartnerSubscribeService : IPartnerSubscribeService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IPeriodicService _periodicService;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public PartnerSubscribeService(IRedisCacheService redisCacheService, IPeriodicService periodicService, IInMemoryUnitOfWork inMemoryUnitOfWork)
        {
            _redisCacheService = redisCacheService;
            _periodicService = periodicService;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        public async Task Subscribe(string channelName)
        {
            await _redisCacheService.SubscribeAsync(channelName, (channel, message) =>
            {
                _periodicService.Enqueue(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        public async Task SubscribeGa28BetKindChannel(string betKindChannel)
        {
            await _redisCacheService.SubscribeAsync(betKindChannel, (channel, message) =>
            {
                InternalProcessGa28BetKind(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        public async Task SubscribeBookieChannel(string bookieChannel)
        {
            await _redisCacheService.SubscribeAsync(bookieChannel, (channel, message) =>
            {
                InternalProcessBookie(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void InternalProcessGa28BetKind(string message)
        {
            var betKind = Newtonsoft.Json.JsonConvert.DeserializeObject<CockFightBetKindModel>(message);
            if (betKind == null || betKind.Id == 0) return;
            var cockFightBetKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICockFightBetKindInMemoryRepository>();
            cockFightBetKindInMemoryRepository.Update(betKind);
        }

        private void InternalProcessBookie(string message)
        {
            var bookieSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<BookieSettingModel>(message);
            if (bookieSetting == null || bookieSetting.Id == 0) return;
            var bookieSettingInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBookieSettingInMemoryRepository>();
            bookieSettingInMemoryRepository.Update(bookieSetting);
        }
    }
}
