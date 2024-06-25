using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.Category;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Enums;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Navigation;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class BuildNavigationService : LotteryBaseService<BuildNavigationService>, IBuildNavigationService
    {
        private delegate List<SubNavigationModel> BuildSubNavigation(List<BetKindModel> betKinds, MatchModel matchModel);
        private readonly Dictionary<Category, BuildSubNavigation> _handlers = new();
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;

        public BuildNavigationService(ILogger<BuildNavigationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            InitHandlers();
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
        }

        private void InitHandlers()
        {
            _handlers[Category.FirstNorthern] = BuildSubNavigationForFirstNorthern;
            _handlers[Category.SecondNorthern] = BuildSubNavigationForSecondNorthern;
            _handlers[Category.Southern18A] = BuildSubNavigationForSouthern;
            _handlers[Category.Southern18A18B] = BuildSubNavigationForSouthern18A18B;
        }

        private List<SubNavigationModel> BuildSubNavigationForSouthern18A18B(List<BetKindModel> betKinds, MatchModel matchModel)
        {
            return new List<SubNavigationModel>();
        }

        private List<SubNavigationModel> BuildSubNavigationForSouthern(List<BetKindModel> betKinds, MatchModel matchModel)
        {
            var categoryId = Category.Southern18A;

            var categoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            var category = categoryInMemoryRepository.FindById(categoryId);
            if (category == null) return new List<SubNavigationModel>();

            var subCategoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISubCategoryInMemoryRepository>();
            var subCategories = subCategoryInMemoryRepository.FindBy(f => f.Category == categoryId).OrderBy(f => f.OrderBy).ToList();
            return CreateSubNavigation(category, subCategories, betKinds, matchModel);
        }

        private List<SubNavigationModel> BuildSubNavigationForSecondNorthern(List<BetKindModel> betKinds, MatchModel matchModel)
        {
            var categoryId = Category.SecondNorthern;

            var categoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            var category = categoryInMemoryRepository.FindById(categoryId);
            if (category == null) return new List<SubNavigationModel>();

            var subCategoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISubCategoryInMemoryRepository>();
            var subCategories = subCategoryInMemoryRepository.FindBy(f => f.Category == categoryId).OrderBy(f => f.OrderBy).ToList();
            return CreateSubNavigation(category, subCategories, betKinds, matchModel);
        }

        private List<SubNavigationModel> BuildSubNavigationForFirstNorthern(List<BetKindModel> betKinds, MatchModel matchModel)
        {
            var categoryId = Category.FirstNorthern;

            var categoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            var category = categoryInMemoryRepository.FindById(categoryId);
            if (category == null) return new List<SubNavigationModel>();

            var subCategoryInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ISubCategoryInMemoryRepository>();
            var subCategories = subCategoryInMemoryRepository.FindBy(f => f.Category == categoryId).OrderBy(f => f.OrderBy).ToList();
            return CreateSubNavigation(category, subCategories, betKinds, matchModel);
        }

        private List<SubNavigationModel> CreateSubNavigation(CategoryModel category, List<SubCategoryModel> subCategories, List<BetKindModel> betKinds, MatchModel matchModel)
        {
            var subNavigation = new List<SubNavigationModel>();
            subCategories.ForEach(f =>
            {
                var itemSubNavigation = new SubNavigationModel
                {
                    Name = f.Name,
                    SubCategoryId = f.Id.ToInt(),
                    Children = new List<SubNavigationDetailModel>()
                };

                foreach (var itemBetKind in f.SubBetKinds)
                {
                    var itemBetKindModel = betKinds.Find(f1 => f1.Id == itemBetKind.ToInt());
                    if (itemBetKindModel == null) continue;

                    int? replacedById = null;
                    int? noOfRemainingNumbers = null;
                    var displayLive = false;
                    var isLive = false;
                    if (matchModel != null && matchModel.MatchResult.TryGetValue(category.Region.ToInt(), out List<ResultByRegionModel> matchResults))
                    {
                        isLive = matchResults.Any(f1 => f1.IsLive);
                        noOfRemainingNumbers = matchResults.FirstOrDefault() != null ? matchResults.FirstOrDefault().NoOfRemainingNumbers : null;
                    }
                    if (isLive && itemBetKindModel.ReplaceByIdWhenLive.HasValue)
                    {
                        itemBetKindModel = betKinds.Find(f1 => f1.Id == itemBetKindModel.ReplaceByIdWhenLive.Value);
                        if (itemBetKindModel == null) continue;
                        displayLive = true;
                        replacedById = itemBetKind.ToInt();
                    }

                    itemSubNavigation.Children.Add(new SubNavigationDetailModel
                    {
                        BetKindId = itemBetKindModel.Id,
                        ReplacedById = replacedById,
                        Name = itemBetKindModel.Name,
                        Display = displayLive,
                        Enabled = itemBetKindModel.Enabled,
                        NoOfRemainingNumbers = noOfRemainingNumbers,
                        Correlations = BuildCorrelationBetKinds(itemBetKindModel.Id.ToEnum<Enums.BetKind>(), betKinds)
                    });
                }

                subNavigation.Add(itemSubNavigation);
            });
            return subNavigation;
        }

        private List<CorrelationBetKindModel> BuildCorrelationBetKinds(Enums.BetKind currentBetKind, List<BetKindModel> betKinds)
        {
            var betKind = betKinds.FirstOrDefault(f => f.Id == currentBetKind.ToInt() && f.IsMixed && f.CorrelationBetKindIds.Count > 0);
            if (betKind == null) return new List<CorrelationBetKindModel>();
            return betKind.CorrelationBetKindIds.Select(f => new CorrelationBetKindModel
            {
                BetKindId = f,
                Name = betKinds.First(f1 => f1.Id == f).Name
            }).ToList();
        }

        public List<SubNavigationModel> GetChildrenHandler(Category category, List<BetKindModel> betKinds, MatchModel runningMatch)
        {
            if (!_handlers.TryGetValue(category, out BuildSubNavigation handler)) return new List<SubNavigationModel>();
            return handler(betKinds, runningMatch);
        }

        public List<Category> GetDiplayCategory()
        {
            return new() { Category.FirstNorthern, Category.SecondNorthern, Category.Southern18A, Category.Southern18A18B };
        }
    }
}
