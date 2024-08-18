using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoService : LotteryBaseService<CasinoService>, ICasinoService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoService(
           ILogger<CasinoService> logger,
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           IClockService clockService,
           ILotteryClientContext clientContext,
           ILotteryUow lotteryUow,
           IPartnerPublishService partnerPublishService,
           IRedisCacheService redisCacheService)
           : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
        }

        public async Task AllBetPlayerLoginAsync(CasinoAllBetPlayerLoginModel model)
        {
            model.PlayerId = ClientContext.Player.PlayerId;
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task CreateAllBetPlayerAsync(CasinoAllBetPlayerModel model)
        {
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task UpdateAllBetPlayerBetSettingAsync(CasinoAllBetPlayerBetSettingModel model)
        {
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task<string> GetGameUrlAsync()
        {
            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            if (casinoPlayerMapping == null) return null;

            var clientUrlKey = casinoPlayerMapping.PlayerId.GetCasinoClientUrlByPlayerId();
            var clientUrlHash = await _redisCacheService.HashGetFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (!clientUrlHash.TryGetValue(clientUrlKey.SubKey, out string gameUrl)) return null;
            return gameUrl;
        }
    }
}