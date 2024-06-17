using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentBetSettings;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Agent
{
    public class AgentBetSettingService : LotteryBaseService<AgentBetSettingService>, IAgentBetSettingService
    {
        private readonly IAuditService _auditService;

        public AgentBetSettingService(ILogger<AgentBetSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow, IAuditService auditService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _auditService = auditService;
        }

        public async Task<GetAgentBetSettingsResult> GetAgentBetSettings()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddsRepository>();

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
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddsRepository>();

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
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddsRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();

            var targetAgentId = ClientContext.Agent.ParentId != 0L ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var auditBetSettings = new List<AuditSettingData>();
            var updateBetKindIds = updateItems.Select(x => x.BetKindId).ToList();
            var updatedBetKinds = await betKindRepos.FindQueryBy(x => updateBetKindIds.Contains(x.Id)).ToListAsync();
            var existedAgentBetSettings = await agentOddRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            existedAgentBetSettings.ForEach(item =>
            {
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if(updateItem != null)
                {
                    var oldBuyValue = item.Buy;
                    var oldMinBetValue = item.MinBet;
                    var oldMaxBetValue = item.MaxBet;
                    var oldMaxPerNumber = item.MaxPerNumber;
                    item.Buy = updateItem.ActualBuy;
                    item.MinBet = updateItem.ActualMinBet;
                    item.MaxBet = updateItem.ActualMaxBet;
                    item.MaxPerNumber = updateItem.ActualMaxPerNumber;

                    auditBetSettings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MinBetTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMinBetValue,
                            NewValue = item.MinBet
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MaxBetTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMaxBetValue,
                            NewValue = item.MaxBet
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MaxPerNumberTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMaxPerNumber,
                            NewValue = item.MaxPerNumber
                        }
                    });
                }
            });

            await LotteryUow.SaveChangesAsync();

            if (existedAgentBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Setting,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = clientAgent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = GetAuditSupermasterId(clientAgent),
                    MasterId = GetAuditMasterId(clientAgent),
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }
        }
        private long GetAuditMasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Agent ? targetUser.MasterId : 0;
        }

        private long GetAuditSupermasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Master or (int)Role.Agent ? targetUser.SupermasterId : 0;
        }
    }
}
