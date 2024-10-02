using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoGameTypeService : LotteryBaseService<CasinoGameTypeService>, ICasinoGameTypeService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoGameTypeService(
            ILogger<CasinoGameTypeService> logger,
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

        public async Task<IEnumerable<CasinoGameType>> GetGameTypesAsync(string category)
        {
            var cAGameTypeService = LotteryUow.GetRepository<ICasinoGameTypeRepository>();
            return await cAGameTypeService.FindByAsync(c => c.Category == category);
        }

        public async Task<IEnumerable<CasinoGameType>> GetAllGameTypesAsync()
        {
            var cAGameTypeService = LotteryUow.GetRepository<ICasinoGameTypeRepository>();
            return cAGameTypeService.GetAll();
        }

    }
}
