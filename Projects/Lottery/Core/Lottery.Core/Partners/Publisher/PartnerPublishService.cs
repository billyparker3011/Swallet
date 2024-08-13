using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models;
using Newtonsoft.Json;

namespace Lottery.Core.Partners.Publish
{
    public class PartnerPublishService : IPartnerPublishService
    {
        private readonly IRedisCacheService _redisCacheService;

        public PartnerPublishService(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        public async Task Publish(IBaseMessageModel model)
        {
            switch (model.Partner)
            {
                case Enums.Partner.PartnerType.GA28:
                    await _redisCacheService.PublishAsync(Configs.PartnerChannelConfigs.Ga28Channel, JsonConvert.SerializeObject(model, CommonHelper.CreateJsonSerializerSettings()));
                    return;
                case Enums.Partner.PartnerType.Allbet:
                    await _redisCacheService.PublishAsync(Configs.PartnerChannelConfigs.AlibetChannel, JsonConvert.SerializeObject(model, CommonHelper.CreateJsonSerializerSettings()));
                    return;
                default:
                    return;
            }
        }
    }
}
