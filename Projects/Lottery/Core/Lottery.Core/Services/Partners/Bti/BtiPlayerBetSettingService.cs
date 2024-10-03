using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Bti;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiPlayerBetSettingService : LotteryBaseService<BtiPlayerBetSettingService>, IBtiPlayerBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;

        public BtiPlayerBetSettingService(
            ILogger<BtiPlayerBetSettingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService,
            IAuditService auditService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _auditService = auditService;
        }

        public async Task<BtiPlayerBetSettingModel> FindAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();
            var item = await repos.FindQueryBy(c => c.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return PrepareModel(item);
        }

        public async Task<IEnumerable<BtiPlayerBetSettingModel>> GetsAsync(long playerId)
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();
            var items = await repos.FindQueryBy(c => c.PlayerId == playerId).ToListAsync();
            var results = new List<BtiPlayerBetSettingModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task<IEnumerable<BtiPlayerBetSettingModel>> GetAllAsync()
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();
            var items = await repos.FindQuery().ToListAsync();
            var results = new List<BtiPlayerBetSettingModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateAsync(BtiPlayerBetSettingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();
            var item = new BtiPlayerBetSetting()
            {
                PlayerId = model.PlayerId,
                BetKindId = model.BetKindId,
                MinBet = model.MinBet,
                MaxBet = model.MaxBet,
                MaxWin = model.MaxWin,
                MaxLoss = model.MaxLoss,
                IsSynchronized = false,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
            };

            await repos.AddAsync(item);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAsync(BtiPlayerBetSettingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();

            var item = await repos.FindByIdAsync(model.Id) ?? throw new NotFoundException();

            item.PlayerId = model.PlayerId;
            item.BetKindId = model.BetKindId;
            item.MinBet = model.MinBet;
            item.MaxBet = model.MaxBet;
            item.MaxWin = model.MaxWin;
            item.MaxLoss = model.MaxLoss;
            item.IsSynchronized = false;
            item.UpdatedAt = DateTime.Now;

            repos.Update(item);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiPlayerBetSettingRepository>();

            repos.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        private BtiPlayerBetSettingModel PrepareModel(BtiPlayerBetSetting item)
        {
            return new BtiPlayerBetSettingModel()
            {
                Id = item.Id,
                PlayerId = item.PlayerId,
                BetKindId = item.BetKindId,
                MinBet = item.MinBet,
                MaxBet = item.MaxBet,
                MaxWin = item.MaxWin,
                MaxLoss = item.MaxLoss,
                IsSynchronized = item.IsSynchronized,

            };
        }
    }
}
