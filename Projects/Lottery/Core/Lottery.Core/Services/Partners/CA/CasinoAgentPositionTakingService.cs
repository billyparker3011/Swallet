using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentPositionTakingService : LotteryBaseService<CasinoAgentPositionTakingService>, ICasinoAgentPositionTakingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoAgentPositionTakingService(
            ILogger<CasinoAgentPositionTakingService> logger,
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

        public async Task<CasinoAgentPositionTaking> FindAgentPositionTakingAsync(long id)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            return await cAAgentPositionTakingRepository.FindByIdAsync(id);
        }

        public async Task<IEnumerable<CasinoAgentPositionTaking>> GetAgentPositionTakingsAsync(long agentId)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            return await cAAgentPositionTakingRepository.FindByAsync(c => c.AgentId == agentId);
        }

        public async Task<IEnumerable<CasinoAgentPositionTaking>> GetAllAgentPositionTakingsAsync()
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            return cAAgentPositionTakingRepository.GetAll();
        }

        public async Task CreateAgentPositionTakingAsync(CreateCasinoAgentPositionTakingModel model)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            var cAAgentPositionTaking = new CasinoAgentPositionTaking()
            {
                AgentId = model.AgentId,
                BetKindId = model.BetKindId,
                PositionTaking = model.PositionTaking,
                CreatedAt = DateTime.Now,
            };

            await cAAgentPositionTakingRepository.AddAsync(cAAgentPositionTaking);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            var cAAgentPositionTaking = await cAAgentPositionTakingRepository.FindByIdAsync(model.Id);

            cAAgentPositionTaking.AgentId = model.AgentId;
            cAAgentPositionTaking.BetKindId = model.BetKindId;
            cAAgentPositionTaking.PositionTaking = model.PositionTaking;
            cAAgentPositionTaking.UpdatedAt = DateTime.Now;


            cAAgentPositionTakingRepository.Update(cAAgentPositionTaking);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task DeleteAgentPositionTakingAsync(long id)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            cAAgentPositionTakingRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }
    }
}
