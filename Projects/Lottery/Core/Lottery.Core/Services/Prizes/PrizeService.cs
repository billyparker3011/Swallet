using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.Prize;
using Lottery.Core.Repositories.Prize;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Prizes
{
    public class PrizeService : LotteryBaseService<PrizeService>, IPrizeService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IPublishCommonService _publishCommonService;

        public PrizeService(ILogger<PrizeService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _publishCommonService = publishCommonService;
        }

        public async Task<List<PrizeModel>> GetPrizes(int? regionId)
        {
            var prizeRepository = LotteryUow.GetRepository<IPrizeRepository>();
            var query = prizeRepository.FindQuery();
            if (regionId.HasValue) query = query.Where(f => f.RegionId == regionId.Value);
            var data = await query.OrderBy(f => f.RegionId).ThenBy(f => f.Id).ToListAsync();
            return data.Select(f => new PrizeModel
            {
                Id = f.Id,
                PrizeId = f.PrizeId,
                RegionId = f.RegionId,
                Name = f.Name,
                NoOfNumbers = f.NoOfNumbers,
                Order = f.Order
            }).ToList();
        }

        public PrizeFilterOptionModel GetFilterOptions()
        {
            var regionRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            return new PrizeFilterOptionModel
            {
                Regions = regionRepository.GetAll()
            };
        }

        public async Task UpdatePrizes(UpdatePrizesModel model)
        {
            var prizeIds = model.Items.Select(f => f.Id).ToList();
            var prizeRepository = LotteryUow.GetRepository<IPrizeRepository>();
            var prizes = await prizeRepository.FindQueryBy(f => prizeIds.Contains(f.Id)).ToListAsync();
            var publishedPrizes = new List<PrizeModel>();
            prizes.ForEach(f =>
            {
                var currentPrize = model.Items.FirstOrDefault(f1 => f1.Id == f.Id);
                if (currentPrize == null) return;

                f.Name = currentPrize.Name;
                f.Order = currentPrize.Order;
                prizeRepository.Update(f);

                publishedPrizes.Add(new PrizeModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    RegionId = f.RegionId,
                    NoOfNumbers = f.NoOfNumbers,
                    Order = f.Order,
                    PrizeId = f.PrizeId
                });
            });

            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishPrize(publishedPrizes);
        }
    }
}
