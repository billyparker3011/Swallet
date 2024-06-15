using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentPositionTaking;
using Lottery.Core.Models.PositionTakings;
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
    public class AgentPositionTakingService : LotteryBaseService<AgentPositionTakingService>, IAgentPositionTakingService
    {
        private readonly IAuditService _auditService;

        public AgentPositionTakingService(ILogger<AgentPositionTakingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow, IAuditService auditService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _auditService = auditService;
        }

        public async Task<List<AgentPositionTakingModel>> GetAgentPositionTakingByAgentIds(int betKindId, List<long> agentIds)
        {
            var agentPositionTakingRepository = LotteryUow.GetRepository<IAgentPositionTakingRepository>();
            return await agentPositionTakingRepository.FindQueryBy(f => f.BetKindId == betKindId && agentIds.Contains(f.AgentId)).Select(f => new AgentPositionTakingModel
            {
                AgentId = f.AgentId,
                BetKindId = f.BetKindId,
                PositionTaking = f.PositionTaking
            }).ToListAsync();
        }

        public async Task<Dictionary<int, List<AgentPositionTakingModel>>> GetAgentPositionTakingByAgentIds(List<int> betKindIds, List<long> agentIds)
        {
            var agentPositionTakingRepository = LotteryUow.GetRepository<IAgentPositionTakingRepository>();
            var positionTakings = await agentPositionTakingRepository.FindQueryBy(f => betKindIds.Contains(f.BetKindId) && agentIds.Contains(f.AgentId)).ToListAsync();
            var data = new Dictionary<int, List<AgentPositionTakingModel>>();
            foreach (var betKindId in betKindIds)
            {
                data[betKindId] = positionTakings.Where(f => f.BetKindId == betKindId).Select(f => new AgentPositionTakingModel
                {
                    AgentId = f.AgentId,
                    BetKindId = f.BetKindId,
                    PositionTaking = f.PositionTaking
                }).ToList();
            }
            return data;
        }

        public async Task<GetAgentPositionTakingResult> GetAgentPositionTakings()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentPositionTakingRepos = LotteryUow.GetRepository<IAgentPositionTakingRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId);
            if (clientAgent is null) throw new NotFoundException();

            var isSubAgent = clientAgent.ParentId != 0;
            long targetAgentId = isSubAgent ? clientAgent.ParentId : clientAgent.AgentId;
            long roleId = clientAgent.RoleId;

            var agentPositionTakings = await agentPositionTakingRepos.FindQuery()
                                                        .Include(x => x.Agent)
                                                        .Include(x => x.BetKind)
                                                        .Where(x => x.Agent.AgentId == targetAgentId
                                                                    && x.Agent.RoleId == roleId
                                                                    && x.BetKind.Id != 1000)
                                                        .OrderBy(x => x.BetKind.RegionId)
                                                        .ThenBy(x => x.BetKind.CategoryId)
                                                        .ThenBy(x => x.BetKind.OrderInCategory)
                                                        .Select(x => new AgentPositionTakingDto
                                                        {
                                                            BetKindId = x.BetKind.Id,
                                                            RegionId = x.BetKind.RegionId,
                                                            CategoryId = x.BetKind.CategoryId,
                                                            BetKindName = x.BetKind.Name,
                                                            DefaultPositionTaking = x.PositionTaking,
                                                            ActualPositionTaking = x.PositionTaking
                                                        })
                                                        .ToListAsync();
            foreach(var pt in agentPositionTakings)
            {
                pt.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)pt.RegionId)?.Name;
                pt.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)pt.CategoryId)?.Name;
            }
            return new GetAgentPositionTakingResult
            {
                AgentPositionTakings = agentPositionTakings
            };
        }

        public async Task<GetAgentPositionTakingResult> GetDetailAgentPositionTakings(long agentId)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentPositionTakingRepos = LotteryUow.GetRepository<IAgentPositionTakingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var defaultPositionTakings = new List<AgentPositionTaking>();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultPositionTakings = await agentPositionTakingRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();
                    break;
                case (int)Role.Master:
                    defaultPositionTakings = await agentPositionTakingRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).ToListAsync();
                    break;
                case (int)Role.Agent:
                    defaultPositionTakings = await agentPositionTakingRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).ToListAsync();
                    break;
            }

            var agentPositionTakings = await agentPositionTakingRepos.FindQuery()
                                                        .Include(x => x.Agent)
                                                        .Include(x => x.BetKind)
                                                        .Where(x => x.Agent.AgentId == targetAgent.AgentId
                                                                    && x.Agent.RoleId == targetAgent.RoleId
                                                                    && x.BetKind.Id != 1000)
                                                        .OrderBy(x => x.BetKind.RegionId)
                                                        .ThenBy(x => x.BetKind.CategoryId)
                                                        .ThenBy(x => x.BetKind.OrderInCategory)
                                                        .Select(x => new AgentPositionTakingDto
                                                        {
                                                            BetKindId = x.BetKind.Id,
                                                            RegionId = x.BetKind.RegionId,
                                                            CategoryId = x.BetKind.CategoryId,
                                                            BetKindName = x.BetKind.Name,
                                                            DefaultPositionTaking = x.PositionTaking,
                                                            ActualPositionTaking = x.PositionTaking
                                                        })
                                                        .ToListAsync();
            foreach (var item in agentPositionTakings)
            {
                var defaultBetKindItem = defaultPositionTakings.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                item.DefaultPositionTaking = defaultBetKindItem != null ? defaultBetKindItem.PositionTaking : item.DefaultPositionTaking;
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return new GetAgentPositionTakingResult
            {
                AgentPositionTakings = agentPositionTakings
            };
        }

        public async Task UpdateAgentPositionTakings(List<AgentPositionTakingDto> updateItems)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentPtRepos = LotteryUow.GetRepository<IAgentPositionTakingRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            
            var targetAgentId = ClientContext.Agent.ParentId != 0L ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != Role.Company.ToInt()) return;

            var auditPositionTakings = new List<AuditSettingData>();
            var updateBetKindIds = updateItems.Select(x => x.BetKindId);
            var updatedBetKinds = await betKindRepos.FindQueryBy(x => updateBetKindIds.Contains(x.Id)).ToListAsync();
            var existedAgentPositionTakings = await agentPtRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            existedAgentPositionTakings.ForEach(item =>
            {
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if (updateItem != null)
                {
                    var oldPTValue = item.PositionTaking;
                    item.PositionTaking = updateItem.ActualPositionTaking;

                    auditPositionTakings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.PositionTakingTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldPTValue,
                            NewValue = item.PositionTaking
                        }
                    });
                }
            });

            await LotteryUow.SaveChangesAsync();

            if (existedAgentPositionTakings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Setting,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = clientAgent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdatePositionTaking,
                    SupermasterId = GetAuditSupermasterId(clientAgent),
                    MasterId = GetAuditMasterId(clientAgent),
                    AuditSettingDatas = auditPositionTakings.OrderBy(x => x.BetKind).ToList()
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
