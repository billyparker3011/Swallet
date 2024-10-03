using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Category;
using Lottery.Core.Models.Navigation;
using Lottery.Core.Services.Match;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class PlayerNavigationService : LotteryBaseService<PlayerNavigationService>, IPlayerNavigationService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IRunningMatchService _runningMatchService;
        private readonly IBuildNavigationService _buildNavigationService;

        public PlayerNavigationService(ILogger<PlayerNavigationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow, IInMemoryUnitOfWork inMemoryUnitOfWork,
            IRunningMatchService runningMatchService,
            IBuildNavigationService buildNavigationService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _runningMatchService = runningMatchService;
            _buildNavigationService = buildNavigationService;
        }

        public async Task<List<NavigationModel>> MyNavigation()
        {
            var data = new List<NavigationModel>();

            var runningMatch = await _runningMatchService.GetRunningMatch();

            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKinds = betKindInMemoryRepository.FindBy(f => true).ToList();

            var displayCategory = _buildNavigationService.GetDisplayCategory();

            var categoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            var categories = categoryInMemoryRepository.FindBy(f => displayCategory.Contains(f.Id)).OrderBy(f => f.OrderBy).ToList();
            categories.ForEach(f =>
            {
                data.Add(new NavigationModel
                {
                    CategoryId = f.Id.ToInt(),
                    Name = f.Name,
                    Code = f.Code,
                    Children = _buildNavigationService.GetChildrenHandler(f.Id, betKinds, runningMatch)
                });
            });
            return data;
        }
    }
}
