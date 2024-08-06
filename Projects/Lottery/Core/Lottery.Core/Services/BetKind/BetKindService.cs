using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.BetKind;
using Lottery.Core.Enums;
using Lottery.Core.Helpers.Converters.BetKinds;
using Lottery.Core.Helpers.Converters.Odds;
using Lottery.Core.InMemory.Category;
using Lottery.Core.InMemory.Region;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
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

        public async Task<List<BetKindSettingModel>> GetBetKinds(int? regionId, int? categoryId)
        {
            var betKindSettings = new List<BetKindSettingModel>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var agentBetSettingRepos = LotteryUow.GetRepository<IAgentOddsRepository>();
            var result = betKindRepos.FindQueryBy(x => !x.IsMixed);
            if (regionId.HasValue)
            {
                result = result.Where(x => x.RegionId == regionId.Value);
            }
            if (categoryId.HasValue)
            {
                result = result.Where(x => x.CategoryId == categoryId.Value);
            }
            var masterBetKinds = await result.OrderBy(x => x.RegionId).ThenBy(x => x.CategoryId).ThenBy(x => x.OrderInCategory).ThenBy(x => x.Id).Select(x => x.ToBetKindModel()).ToListAsync();
            var companyBetKinds = await agentBetSettingRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.ParentId == 0L && x.Agent.RoleId == Role.Company.ToInt()).ToListAsync();
            foreach (var item in masterBetKinds)
            {
                var targetCompanyBetKind = companyBetKinds.FirstOrDefault(x => x.BetKindId == item.Id);
                if (targetCompanyBetKind == null) continue;
                betKindSettings.Add(new BetKindSettingModel
                {
                    Id = item.Id,
                    RegionId = item.RegionId,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    Name = item.Name,
                    Award = item.Award,
                    CorrelationBetKindIds = item.CorrelationBetKindIds,
                    Enabled = item.Enabled,
                    IsLive = item.IsLive,
                    IsMixed = item.IsMixed,
                    OrderInCategory = item.OrderInCategory,
                    ReplaceByIdWhenLive = item.ReplaceByIdWhenLive,
                    MinBuy = targetCompanyBetKind.MinBuy,
                    ActualBuy = targetCompanyBetKind.Buy,
                    MaxBuy = targetCompanyBetKind.MaxBuy,
                });
            }
            return betKindSettings;
        }

        public GetFilterDataDto GetFilterDatas()
        {
            var regionInMemory = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var categoryInMemory = _inMemoryUnitOfWork.GetRepository<ICategoryInMemoryRepository>();
            return new GetFilterDataDto
            {
                Regions = regionInMemory.GetAll(),
                Categories = categoryInMemory.GetAll()
            };
        }

        public Enums.BetKind GetReplacedBetKind(Enums.BetKind betKind)
        {
            if (betKind.Is(Enums.BetKind.FirstNorthern_Northern_LoLive)) return Enums.BetKind.FirstNorthern_Northern_Lo;
            if (betKind.Is(Enums.BetKind.Central_2D18LoLive)) return Enums.BetKind.Central_2D18Lo;
            if (betKind.Is(Enums.BetKind.Southern_2D18LoLive)) return Enums.BetKind.Southern_2D18Lo;
            throw new NotFoundException();
        }

        public async Task UpdateBetKinds(List<BetKindSettingModel> updatedItems)
        {
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentBetSettingRepos = LotteryUow.GetRepository<IAgentOddsRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var playerBetSettingRepos = LotteryUow.GetRepository<IPlayerOddsRepository>();

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

            // Update buy setting of company
            var targetCompanyBetSettings = await agentBetSettingRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.ParentId == 0L && x.Agent.RoleId == Role.Company.ToInt() && requestedBetKindIds.Contains(x.BetKindId)).ToListAsync();

            var childAgentIds = await agentRepos.FindQueryBy(x => x.RoleId > Role.Company.ToInt() && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var childPlayerIds = await playerRepos.FindQuery().Select(x => x.PlayerId).ToListAsync();
            var existedChildAgentBetSettings = await agentBetSettingRepos.FindQuery().Include(x => x.Agent).Where(x => childAgentIds.Contains(x.AgentId) && requestedBetKindIds.Contains(x.BetKindId)).ToListAsync();
            var existedChildPlayerBetSettings = await playerBetSettingRepos.FindQuery().Include(x => x.Player).Where(x => childPlayerIds.Contains(x.PlayerId) && requestedBetKindIds.Contains(x.BetKindId)).ToListAsync();
            targetCompanyBetSettings.ForEach(bs =>
            {
                var updatedChildAgentItems = existedChildAgentBetSettings.Where(x => x.BetKindId == bs.BetKindId).OrderBy(x => x.Agent.RoleId).ThenBy(x => x.AgentId).ToList();
                var updatedChildPlayerItems = existedChildPlayerBetSettings.Where(x => x.BetKindId == bs.BetKindId).ToList();
                var updatedItem = updatedItems.FirstOrDefault(x => x.Id == bs.BetKindId);
                if (updatedItem != null)
                {
                    var oldBuyValue = bs.Buy;
                    bs.Buy = updatedItem.ActualBuy;

                    // Update child agent minBuy, actualBuy
                    updatedChildAgentItems.ForEach(childAgentItem =>
                    {
                        AgentOdd parentItem = null;
                        if (childAgentItem.Agent.RoleId == Role.Supermaster.ToInt()) parentItem = bs;
                        else if (childAgentItem.Agent.RoleId == Role.Master.ToInt()) parentItem = updatedChildAgentItems.FirstOrDefault(f => f.AgentId == childAgentItem.Agent.SupermasterId);
                        else if (childAgentItem.Agent.RoleId == Role.Agent.ToInt()) parentItem = updatedChildAgentItems.FirstOrDefault(f => f.AgentId == childAgentItem.Agent.MasterId);
                        if (parentItem == null) return;

                        childAgentItem.MinBuy = parentItem.Buy;
                        if (childAgentItem.MinBuy > childAgentItem.Buy)
                        {
                            childAgentItem.Buy = childAgentItem.MinBuy;
                        }
                    });

                    // Update Child Player MinBuy, ActualBuy
                    updatedChildPlayerItems.ForEach(childPlayerItem =>
                    {
                        var parentAgentItem = updatedChildAgentItems.FirstOrDefault(x => x.AgentId == childPlayerItem.Player.AgentId);
                        if (parentAgentItem == null) return;

                        childPlayerItem.Buy = parentAgentItem.Buy > childPlayerItem.Buy ? parentAgentItem.Buy : childPlayerItem.Buy;
                    });
                }
            });
            await LotteryUow.SaveChangesAsync();

            //  Reload betkind
            var publishedBetKinds = await betKindRepos.FindQueryBy(f => updatedBetKindIds.Contains(f.Id)).Select(f => f.ToBetKindModel()).ToListAsync();
            await _publishCommonService.PublishBetKind(publishedBetKinds);

            //  Reload default Bet Setting of Company
            var defaultBetSettings = await agentBetSettingRepos.FindDefaultOdds();
            await _publishCommonService.PublishDefaultOdds(defaultBetSettings.Select(f => f.ToOddsModel()).ToList());
        }
    }
}
