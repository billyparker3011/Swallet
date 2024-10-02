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
    public class CasinoBetKindService : LotteryBaseService<CasinoBetKindService>, ICasinoBetKindService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoBetKindService(
            ILogger<CasinoBetKindService> logger,
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
        public async Task<CasinoBetKind> FindBetKindAsync(int id)
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            return await cABetKindRepository.FindByIdAsync(id);
        }

        public async Task<IEnumerable<CasinoBetKind>> GetBetKindsAsync(string name)
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            return await cABetKindRepository.FindByAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<CasinoBetKind>> GetAllBetKindsAsync()
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            return cABetKindRepository.GetAll();
        }

        public async Task CreateBetKindAsync(CreateCasinoBetKindModel model)
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();
            var betKind = new CasinoBetKind()
            {
                Name = model.Name,
                Code = model.Code,
                CategoryId = model.CategoryId,
                IsLive = model.IsLive,
                Enabled = model.Enabled,
            };

            await cABetKindRepository.AddAsync(betKind);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateBetKindAsync(UpdateCasinoBetKindModel model)
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();

            var betKind = await cABetKindRepository.FindByIdAsync(model.Id);

            betKind.Name = model.Name;
            betKind.Code = model.Code;
            betKind.CategoryId = model.CategoryId;
            betKind.IsLive = model.IsLive;
            betKind.Enabled = model.Enabled;

            cABetKindRepository.Update(betKind);
            await LotteryUow.SaveChangesAsync();

        }

        public async Task DeleteBetKindAsync(int id)
        {
            var cABetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();

            cABetKindRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }
    }
}
