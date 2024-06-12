using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentBetSettings;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Agent
{
    public class AgentBetSettingService : LotteryBaseService<AgentBetSettingService>, IAgentBetSettingService
    {
        public AgentBetSettingService(ILogger<AgentBetSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetAgentBetSettingsResult> GetAgentBetSettings()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId);
            if (clientAgent is null) throw new NotFoundException();

            var isSubAgent = clientAgent.ParentId != 0;
            long targetAgentId = isSubAgent ? clientAgent.ParentId : clientAgent.AgentId;
            long roleId = clientAgent.RoleId;
            var agentBetSettings = await agentOddRepos.FindQuery()
                                                        .Include(x => x.Agent)
                                                        .Include(x => x.BetKind)
                                                        .Where(x => x.Agent.AgentId == targetAgentId
                                                                    && x.Agent.RoleId == roleId
                                                                    && x.BetKind.Id != 1000)
                                                        .OrderBy(x => x.BetKind.RegionId)
                                                        .ThenBy(x => x.BetKind.CategoryId)
                                                        .ThenBy(x => x.BetKind.OrderInCategory)
                                                        .Select(x => new AgentBetSettingDto
                                                        {
                                                            BetKindId = x.BetKind.Id,
                                                            RegionId = x.BetKind.RegionId,
                                                            CategoryId = x.BetKind.CategoryId,
                                                            BetKindName = x.BetKind.Name,
                                                            MinBuy = x.Buy,
                                                            MaxBuy = x.MaxBuy,
                                                            ActualBuy = x.Buy,
                                                            DefaultMinBet = x.MinBet,
                                                            ActualMinBet = x.MinBet,
                                                            DefaultMaxBet = x.MaxBet,
                                                            ActualMaxBet = x.MaxBet,
                                                            DefaultMaxPerNumber = x.MaxPerNumber,
                                                            ActualMaxPerNumber = x.MaxPerNumber,
                                                        })
                                                        .ToListAsync();
            foreach(var setting in agentBetSettings)
            {
                setting.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)setting.RegionId)?.Name;
                setting.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)setting.CategoryId)?.Name;
            }
            return new GetAgentBetSettingsResult
            {
                AgentBetSettings = agentBetSettings
            };
        }

        public async Task<GetAgentBetSettingsResult> GetDetailAgentBetSettings(long agentId)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var defaultBetSettings = new List<AgentOdd>();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultBetSettings = await agentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();
                    break;
                case (int)Role.Master:
                    defaultBetSettings = await agentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).ToListAsync();
                    break;
                case (int)Role.Agent:
                    defaultBetSettings = await agentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).ToListAsync();
                    break;
            }

            var agentBetSettings = await agentOddRepos.FindQuery()
                                                        .Include(x => x.Agent)
                                                        .Include(x => x.BetKind)
                                                        .Where(x => x.Agent.AgentId == targetAgent.AgentId
                                                                    && x.Agent.RoleId == targetAgent.RoleId
                                                                    && x.BetKind.Id != 1000)
                                                        .OrderBy(x => x.BetKind.RegionId)
                                                        .ThenBy(x => x.BetKind.CategoryId)
                                                        .ThenBy(x => x.BetKind.OrderInCategory)
                                                        .Select(x => new AgentBetSettingDto
                                                        {
                                                            BetKindId = x.BetKind.Id,
                                                            RegionId = x.BetKind.RegionId,
                                                            CategoryId = x.BetKind.CategoryId,
                                                            BetKindName = x.BetKind.Name,
                                                            MinBuy = x.MinBuy,
                                                            MaxBuy = x.MaxBuy,
                                                            ActualBuy = x.Buy,
                                                            DefaultMinBet = x.MinBet,
                                                            ActualMinBet = x.MinBet,
                                                            DefaultMaxBet = x.MaxBet,
                                                            ActualMaxBet = x.MaxBet,
                                                            DefaultMaxPerNumber = x.MaxPerNumber,
                                                            ActualMaxPerNumber = x.MaxPerNumber,
                                                        })
                                                        .ToListAsync();
            foreach(var item in agentBetSettings)
            {
                var defaultBetKindItem = defaultBetSettings.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                item.DefaultMinBet = defaultBetKindItem != null ? defaultBetKindItem.MinBet : item.DefaultMinBet;
                item.DefaultMaxBet = defaultBetKindItem != null ? defaultBetKindItem.MaxBet : item.DefaultMaxBet;
                item.DefaultMaxPerNumber = defaultBetKindItem != null ? defaultBetKindItem.MaxPerNumber : item.DefaultMaxPerNumber;
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return new GetAgentBetSettingsResult
            {
                AgentBetSettings = agentBetSettings
            };
        }

        public async Task UpdateAgentBetSettings(List<AgentBetSettingDto> updateItems)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddRepository>();

            var targetAgentId = ClientContext.Agent.ParentId != 0L ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var updateBetKindIds = updateItems.Select(x => x.BetKindId).ToList();
            var existedAgentBetSettings = await agentOddRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            existedAgentBetSettings.ForEach(item =>
            {
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if(updateItem != null)
                {
                    item.Buy = updateItem.ActualBuy;
                    item.MinBet = updateItem.ActualMinBet;
                    item.MaxBet = updateItem.ActualMaxBet;
                    item.MaxPerNumber = updateItem.ActualMaxPerNumber;
                }
            });

            await LotteryUow.SaveChangesAsync();
        }
    }
}
