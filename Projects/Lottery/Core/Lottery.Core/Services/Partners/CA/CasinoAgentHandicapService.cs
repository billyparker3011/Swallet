using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Services.CockFight;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentHandicapService: LotteryBaseService<CasinoAgentHandicapService>, ICasinoAgentHandicapService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoAgentHandicapService(
            ILogger<CasinoAgentHandicapService> logger,
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

        public async Task<IEnumerable<CasinoAgentHandicap>> GetAgentHandicapsAsync(string type)
        {
            var cAAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            return await cAAgentHandicapRepository.FindByAsync(c => c.Type == type);
        }

        public async Task<IEnumerable<CasinoAgentHandicap>> GetAllAgentHandicapsAsync()
        {
            var cAAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            return cAAgentHandicapRepository.GetAll();
        }

    }
}
