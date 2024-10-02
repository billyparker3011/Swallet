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
    public class BtiBetKindService : LotteryBaseService<BtiBetKindService>, IBtiBetKindService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;

        public BtiBetKindService(
            ILogger<BtiBetKindService> logger,
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

        public async Task<BtiBetKindModel> FindAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();
            var item = await repos.FindQueryBy(c => c.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return PrepareModel(item);
        }

        public async Task<IEnumerable<BtiBetKindModel>> GetsAsync(string name)
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();
            var items = await repos.FindQueryBy(c => c.Name == name).ToListAsync();
            var results = new List<BtiBetKindModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task<IEnumerable<BtiBetKindModel>> GetAllAsync()
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();
            var items = await repos.FindQuery().ToListAsync();
            var results = new List<BtiBetKindModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateAsync(BtiBetKindModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();
            var item = new BtiBetKind()
            {
               Name = model.Name,
               Enabled= model.Enabled,
               BranchId=model.BranchId
            };

            await repos.AddAsync(item);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAsync(BtiBetKindModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();

            var item = await repos.FindByIdAsync(model.Id) ?? throw new NotFoundException();;

            item.Name = model.Name;
            item.Enabled = model.Enabled;
            item.BranchId = model.BranchId;

            repos.Update(item);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiBetKindRepository>();

            repos.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        private BtiBetKindModel PrepareModel(BtiBetKind item)
        {
            return new BtiBetKindModel()
            {
                Id = item.Id,
                Name = item.Name,
                Enabled = item.Enabled,
                BranchId = item.BranchId

            };
        }
    }
}
