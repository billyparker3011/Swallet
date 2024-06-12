using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.BetKind;
using Lottery.Core.Helpers.Converters.BetKinds;
using Lottery.Core.InMemory.Category;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.BetKind
{
    public class BetKindService : LotteryBaseService<BetKindService>, IBetKindService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IPublishCommonService _publishCommonService;

        public BetKindService(ILogger<BetKindService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow, IInMemoryUnitOfWork inMemoryUnitOfWork,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _publishCommonService = publishCommonService;
        }

        public async Task<List<BetKindModel>> GetBetKinds(int? regionId, int? categoryId)
        {
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var result = betKindRepos.FindQueryBy(x => !x.IsMixed);
            if (regionId.HasValue)
            {
                result = result.Where(x => x.RegionId == regionId.Value);
            }
            if (categoryId.HasValue)
            {
                result = result.Where(x => x.CategoryId == categoryId.Value);
            }
            return await result.OrderBy(x => x.RegionId).ThenBy(x => x.CategoryId).ThenBy(x => x.OrderInCategory).ThenBy(x => x.Id).Select(x => x.ToBetKindModel()).ToListAsync();
        }

        public GetFilterDataDto GetFilterDatas()
        {
            //Init repos
            var regionInMemory = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var categoryInMemory = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            return new GetFilterDataDto
            {
                Regions = regionInMemory.GetAll(),
                Categories = categoryInMemory.GetAll()
            };
        }

        public async Task UpdateBetKinds(List<BetKindModel> updatedItems)
        {
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();

            var requestedBetKindIds = updatedItems.Select(x => x.Id);
            var requestedBetKinds = await betKindRepos.FindQueryBy(x => requestedBetKindIds.Contains(x.Id)).ToListAsync();
            var updatedBetKindIds = new List<int>();
            requestedBetKinds.ForEach(item =>
            {
                var updatedItem = updatedItems.FirstOrDefault(x => x.Id == item.Id);
                if (updatedItem == null) return;

                item.Name = updatedItem.Name;
                item.OrderInCategory = updatedItem.OrderInCategory;
                item.Award = updatedItem.Award;
                item.Enabled = updatedItem.Enabled;

                updatedBetKindIds.Add(item.Id);
            });
            await LotteryUow.SaveChangesAsync();

            //  We have some col doesn't change. We need to load again...
            var publishedBetKinds = await betKindRepos.FindQueryBy(f => updatedBetKindIds.Contains(f.Id)).Select(f => f.ToBetKindModel()).ToListAsync();
            await _publishCommonService.PublishBetKind(publishedBetKinds);
        }
    }
}
