using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoPlayerMappingService : LotteryBaseService<CasinoPlayerMappingService>, ICasinoPlayerMappingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoPlayerMappingService(
            ILogger<CasinoPlayerMappingService> logger,
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

        public async Task<CasinoPlayerMapping> FindPlayerMappingAsync(long id)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            return await cAPlayerMappingRepository.FindByIdAsync(id);
        }

        public async Task<CasinoPlayerMapping> FindPlayerMappingByPlayerIdAsync(long playerId)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            return await cAPlayerMappingRepository.FindQueryBy(c=>c.PlayerId == playerId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CasinoPlayerMapping>> GetPlayerMappingsAsync(string username)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            return await cAPlayerMappingRepository.FindByAsync(c => c.Player.Username == username);
        }

        public async Task<IEnumerable<CasinoPlayerMapping>> GetAllPlayerMappingsAsync()
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            return cAPlayerMappingRepository.GetAll();
        }

        public async Task CreatePlayerMappingAsync(CreateCasinoPlayerMappingModel model)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            var cAPlayerMapping = new CasinoPlayerMapping()
            {
                PlayerId = model.PlayerId,
                BookiePlayerId = model.BookiePlayerId,
                NickName = model.NickName,
                IsAccountEnable = model.IsAccountEnable,
                IsAlowedToBet = model.IsAlowedToBet,
                CreatedAt = DateTime.Now,
            };

            await cAPlayerMappingRepository.AddAsync(cAPlayerMapping);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdatePlayerMappingAsync(UpdateCasinoPlayerMappingModel model)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            var cAPlayerMapping = await cAPlayerMappingRepository.FindByIdAsync(model.Id);

            cAPlayerMapping.PlayerId = model.PlayerId;
            cAPlayerMapping.BookiePlayerId = model.BookiePlayerId;
            cAPlayerMapping.NickName = model.NickName;
            cAPlayerMapping.IsAccountEnable = model.IsAccountEnable;
            cAPlayerMapping.IsAlowedToBet = model.IsAlowedToBet;
            cAPlayerMapping.UpdatedAt = DateTime.Now;


            cAPlayerMappingRepository.Update(cAPlayerMapping);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task DeletePlayerMappingAsync(long id)
        {
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            cAPlayerMappingRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }
    }
}
