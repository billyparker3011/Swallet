using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentBetSettingService : LotteryBaseService<CockFightAgentBetSettingService>, ICockFightAgentBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;
        public CockFightAgentBetSettingService(
            ILogger<CockFightAgentBetSettingService> logger,
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

        public async Task<GetCockFightAgentBetSettingResult> GetCockFightAgentBetSettingDetail(long agentId)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cockFightAgentOddRepos = LotteryUow.GetRepository<ICockFightAgentBetSettingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();
            var cockFightCompanyOdds = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();

            var defaultBetSetting = new CockFightAgentBetSetting();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultBetSetting = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).FirstOrDefaultAsync();
                    break;
                case (int)Role.Master:
                    defaultBetSetting = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).FirstOrDefaultAsync();
                    break;
                case (int)Role.Agent:
                    defaultBetSetting = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).FirstOrDefaultAsync();
                    break;
            }

            return await cockFightAgentOddRepos.FindQuery()
                                                .Include(x => x.Agent)
                                                .Include(x => x.CockFightBetKind)
                                                .Where(x => x.Agent.AgentId == targetAgent.AgentId
                                                            && x.Agent.RoleId == targetAgent.RoleId)
                                                .Select(x => new GetCockFightAgentBetSettingResult
                                                {
                                                    BetKindId = x.CockFightBetKind.Id,
                                                    BetKindName = x.CockFightBetKind.Name,
                                                    MainLimitAmountPerFight = x.MainLimitAmountPerFight,
                                                    DefaultMaxMainLimitAmountPerFight = defaultBetSetting != null ? defaultBetSetting.MainLimitAmountPerFight : 0m,
                                                    DrawLimitAmountPerFight = x.DrawLimitAmountPerFight,
                                                    DefaultMaxDrawLimitAmountPerFight = defaultBetSetting != null ? defaultBetSetting.DrawLimitAmountPerFight : 0m,
                                                    LimitNumTicketPerFight = x.LimitNumTicketPerFight,
                                                    DefaultMaxLimitNumTicketPerFight = defaultBetSetting != null ? defaultBetSetting.LimitNumTicketPerFight : 0m
                                                })
                                                .FirstOrDefaultAsync();
        }

        public async Task UpdateCockFightAgentBetSetting(long agentId, UpdateCockFightAgentBetSettingModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightAgentOddRepository = LotteryUow.GetRepository<ICockFightAgentBetSettingRepository>();
            var cockFightPlayerOddRepository = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();
            var cockFightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var agent = await agentRepository.FindByIdAsync(agentId) ?? throw new NotFoundException();
            var auditBetSettings = new List<AuditSettingData>();
            var updatedCockFightBetKind = await cockFightBetKindRepos.FindQueryBy(x => x.Id == model.BetKindId).FirstOrDefaultAsync() ?? throw new NotFoundException();

            var existedCfAgentBetSetting = await cockFightAgentOddRepository.FindQueryBy(x => x.AgentId == agentId && x.BetKindId == updatedCockFightBetKind.Id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var childAgentIds = await GetChildAgentIds(agentRepository, agent);
            var childPlayerIds = await GetChildPlayerIds(playerRepository, agent);
            var updatedChildCfAgentBetSettings = await cockFightAgentOddRepository.FindQuery().Include(x => x.Agent).Where(x => childAgentIds.Contains(x.AgentId) && x.BetKindId == updatedCockFightBetKind.Id).OrderBy(x => x.Agent.RoleId).ThenBy(x => x.AgentId).ToListAsync();
            var updatedChildCfPlayerBetSettings = await cockFightPlayerOddRepository.FindQuery().Include(x => x.Player).Where(x => childPlayerIds.Contains(x.PlayerId) && x.BetKindId == updatedCockFightBetKind.Id).ToListAsync();

            // Update target cockfight agent bet setting
            var oldMainLimitAmountPerFight = existedCfAgentBetSetting.MainLimitAmountPerFight;
            var oldDrawLimitAmountPerFight = existedCfAgentBetSetting.DrawLimitAmountPerFight;
            var oldLimitNumTicketPerFight = existedCfAgentBetSetting.LimitNumTicketPerFight;
            existedCfAgentBetSetting.MainLimitAmountPerFight = model.MainLimitAmountPerFight;
            existedCfAgentBetSetting.DrawLimitAmountPerFight = model.DrawLimitAmountPerFight;
            existedCfAgentBetSetting.LimitNumTicketPerFight = model.LimitNumTicketPerFight;

            // Update all children of target agent if new value of agent is lower than the oldest one
            // Update child cockfight agent
            UpdateChildCockFightAgentBetSetting(existedCfAgentBetSetting, updatedChildCfAgentBetSettings);

            // Update child cockfight player
            var childCfPlayerMappings = await cockFightPlayerMappingRepos.FindQueryBy(x => childPlayerIds.Contains(x.PlayerId)).ToListAsync();
            UpdateChildCockFightPlayerBetSetting(existedCfAgentBetSetting, updatedChildCfAgentBetSettings, updatedChildCfPlayerBetSettings, childCfPlayerMappings);

            if (oldMainLimitAmountPerFight != existedCfAgentBetSetting.MainLimitAmountPerFight || oldDrawLimitAmountPerFight != existedCfAgentBetSetting.DrawLimitAmountPerFight || oldLimitNumTicketPerFight != existedCfAgentBetSetting.LimitNumTicketPerFight)
            {
                auditBetSettings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightMainLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldMainLimitAmountPerFight,
                            NewValue = existedCfAgentBetSetting.MainLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightDrawLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldDrawLimitAmountPerFight,
                            NewValue = existedCfAgentBetSetting.DrawLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightLimitNumTicketPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldLimitNumTicketPerFight,
                            NewValue = existedCfAgentBetSetting.LimitNumTicketPerFight
                        }
                    });
            }
            await LotteryUow.SaveChangesAsync();

            if (auditBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = agent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = AuditDataHelper.GetAuditSupermasterId(agent),
                    MasterId = AuditDataHelper.GetAuditMasterId(agent),
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }
        }

        private void UpdateChildCockFightPlayerBetSetting(CockFightAgentBetSetting existedCfAgentBetSetting, List<CockFightAgentBetSetting> updatedChildCfAgentBetSettings, List<CockFightPlayerBetSetting> updatedChildCfPlayerBetSettings, List<CockFightPlayerMapping> childCfPlayerMappings)
        {
            updatedChildCfPlayerBetSettings.ForEach(async childCfPlayerItem =>
            {
                var parentAgentItem = existedCfAgentBetSetting.Agent.RoleId == Role.Agent.ToInt() ? existedCfAgentBetSetting : updatedChildCfAgentBetSettings.FirstOrDefault(x => x.AgentId == childCfPlayerItem.Player.AgentId);
                if (parentAgentItem == null) return;

                childCfPlayerItem.MainLimitAmountPerFight = parentAgentItem.MainLimitAmountPerFight < childCfPlayerItem.MainLimitAmountPerFight ? parentAgentItem.MainLimitAmountPerFight : childCfPlayerItem.MainLimitAmountPerFight;
                childCfPlayerItem.DrawLimitAmountPerFight = parentAgentItem.DrawLimitAmountPerFight < childCfPlayerItem.DrawLimitAmountPerFight ? parentAgentItem.DrawLimitAmountPerFight : childCfPlayerItem.DrawLimitAmountPerFight;
                childCfPlayerItem.LimitNumTicketPerFight = parentAgentItem.LimitNumTicketPerFight < childCfPlayerItem.LimitNumTicketPerFight ? parentAgentItem.LimitNumTicketPerFight : childCfPlayerItem.LimitNumTicketPerFight;

                // Sync child player setting to Ga28
                var targetChildCfPlayerMapping = childCfPlayerMappings.FirstOrDefault(x => x.PlayerId == childCfPlayerItem.PlayerId && x.IsInitial && !x.IsFreeze && x.IsEnabled);
                if (targetChildCfPlayerMapping == null) return;

                targetChildCfPlayerMapping.NeedsRecalcBetSetting = true;
            });
        }

        private void UpdateChildCockFightAgentBetSetting(CockFightAgentBetSetting existedCfAgentBetSetting, List<CockFightAgentBetSetting> updatedChildCfAgentBetSettings)
        {
            updatedChildCfAgentBetSettings.ForEach(childCfAgentItem =>
            {
                CockFightAgentBetSetting parentItem = null;
                if (existedCfAgentBetSetting.Agent.RoleId == Role.Company.ToInt())
                {
                    if (childCfAgentItem.Agent.RoleId == Role.Supermaster.ToInt()) parentItem = existedCfAgentBetSetting;
                    else if (childCfAgentItem.Agent.RoleId == Role.Master.ToInt()) parentItem = updatedChildCfAgentBetSettings.FirstOrDefault(f => f.AgentId == childCfAgentItem.Agent.SupermasterId);
                    else if (childCfAgentItem.Agent.RoleId == Role.Agent.ToInt()) parentItem = updatedChildCfAgentBetSettings.FirstOrDefault(f => f.AgentId == childCfAgentItem.Agent.MasterId);
                    if (parentItem == null) return;
                }
                if (existedCfAgentBetSetting.Agent.RoleId == Role.Supermaster.ToInt())
                {
                    if (childCfAgentItem.Agent.RoleId == Role.Master.ToInt()) parentItem = existedCfAgentBetSetting;
                    else if (childCfAgentItem.Agent.RoleId == Role.Agent.ToInt()) parentItem = updatedChildCfAgentBetSettings.FirstOrDefault(f => f.AgentId == childCfAgentItem.Agent.MasterId);
                    if (parentItem == null) return;
                }
                else if (existedCfAgentBetSetting.Agent.RoleId == Role.Master.ToInt())
                {
                    if (childCfAgentItem.Agent.RoleId == Role.Agent.ToInt()) parentItem = existedCfAgentBetSetting;
                    if (parentItem == null) return;
                }
                else if (existedCfAgentBetSetting.Agent.RoleId == Role.Agent.ToInt())
                {
                    return;
                }

                childCfAgentItem.MainLimitAmountPerFight = parentItem.MainLimitAmountPerFight < childCfAgentItem.MainLimitAmountPerFight ? parentItem.MainLimitAmountPerFight : childCfAgentItem.MainLimitAmountPerFight;
                childCfAgentItem.DrawLimitAmountPerFight = parentItem.DrawLimitAmountPerFight < childCfAgentItem.DrawLimitAmountPerFight ? parentItem.DrawLimitAmountPerFight : childCfAgentItem.DrawLimitAmountPerFight;
                childCfAgentItem.LimitNumTicketPerFight = parentItem.LimitNumTicketPerFight < childCfAgentItem.LimitNumTicketPerFight ? parentItem.LimitNumTicketPerFight : childCfAgentItem.LimitNumTicketPerFight;
            });
        }

        private async Task<List<long>> GetChildPlayerIds(IPlayerRepository playerRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Supermaster:
                    return await playerRepository.FindQueryBy(x => x.SupermasterId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                case (int)Role.Master:
                    return await playerRepository.FindQueryBy(x => x.MasterId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                case (int)Role.Agent:
                    return await playerRepository.FindQueryBy(x => x.AgentId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                default:
                    return new List<long>();
            }
        }

        private async Task<List<long>> GetChildAgentIds(IAgentRepository agentRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Supermaster:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.SupermasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                case (int)Role.Master:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.MasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                default:
                    return new List<long>();
            }
        }

        public async Task<GetCockFightAgentBetSettingResult> GetDefaultCockFightCompanyBetSetting()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cockFightAgentOddRepos = LotteryUow.GetRepository<ICockFightAgentBetSettingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (targetAgent.RoleId != Role.Company.ToInt()) return new GetCockFightAgentBetSettingResult();
            var defaultCockFightCompanyOdds = await cockFightAgentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();

            return await cockFightAgentOddRepos.FindQuery()
                                                .Include(x => x.Agent)
                                                .Include(x => x.CockFightBetKind)
                                                .Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L)
                                                .Select(x => new GetCockFightAgentBetSettingResult
                                                {
                                                    BetKindId = x.CockFightBetKind.Id,
                                                    BetKindName = x.CockFightBetKind.Name,
                                                    MainLimitAmountPerFight = x.MainLimitAmountPerFight,
                                                    DrawLimitAmountPerFight = x.DrawLimitAmountPerFight,
                                                    LimitNumTicketPerFight = x.LimitNumTicketPerFight
                                                })
                                                .FirstOrDefaultAsync();
        }

        public async Task UpdateDefaultCockFightCompanyBetSetting(UpdateCockFightAgentBetSettingModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightAgentOddRepository = LotteryUow.GetRepository<ICockFightAgentBetSettingRepository>();
            var cockFightPlayerOddRepository = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();
            var cockFightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var agent = await agentRepository.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (agent.RoleId != Role.Company.ToInt()) return;

            var auditBetSettings = new List<AuditSettingData>();
            var updatedCockFightBetKind = await cockFightBetKindRepos.FindQueryBy(x => x.Id == model.BetKindId).FirstOrDefaultAsync() ?? throw new NotFoundException();

            var existedCfAgentBetSetting = await cockFightAgentOddRepository.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L && x.BetKindId == updatedCockFightBetKind.Id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var childAgentIds = await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var childPlayerIds = await playerRepository.FindQuery().Select(x => x.PlayerId).ToListAsync();
            var updatedChildCfAgentBetSettings = await cockFightAgentOddRepository.FindQuery().Include(x => x.Agent).Where(x => childAgentIds.Contains(x.AgentId) && x.BetKindId == updatedCockFightBetKind.Id).OrderBy(x => x.Agent.RoleId).ThenBy(x => x.AgentId).ToListAsync();
            var updatedChildCfPlayerBetSettings = await cockFightPlayerOddRepository.FindQuery().Include(x => x.Player).Where(x => childPlayerIds.Contains(x.PlayerId) && x.BetKindId == updatedCockFightBetKind.Id).ToListAsync();

            // Update target cockfight agent bet setting
            var oldMainLimitAmountPerFight = existedCfAgentBetSetting.MainLimitAmountPerFight;
            var oldDrawLimitAmountPerFight = existedCfAgentBetSetting.DrawLimitAmountPerFight;
            var oldLimitNumTicketPerFight = existedCfAgentBetSetting.LimitNumTicketPerFight;
            existedCfAgentBetSetting.MainLimitAmountPerFight = model.MainLimitAmountPerFight;
            existedCfAgentBetSetting.DrawLimitAmountPerFight = model.DrawLimitAmountPerFight;
            existedCfAgentBetSetting.LimitNumTicketPerFight = model.LimitNumTicketPerFight;

            // Update all children of target agent if new value of agent is lower than the oldest one
            // Update child cockfight agent
            UpdateChildCockFightAgentBetSetting(existedCfAgentBetSetting, updatedChildCfAgentBetSettings);

            // Update child cockfight player
            var childCfPlayerMappings = await cockFightPlayerMappingRepos.FindQueryBy(x => childPlayerIds.Contains(x.PlayerId)).ToListAsync();
            UpdateChildCockFightPlayerBetSetting(existedCfAgentBetSetting, updatedChildCfAgentBetSettings, updatedChildCfPlayerBetSettings, childCfPlayerMappings);

            if (oldMainLimitAmountPerFight != existedCfAgentBetSetting.MainLimitAmountPerFight || oldDrawLimitAmountPerFight != existedCfAgentBetSetting.DrawLimitAmountPerFight || oldLimitNumTicketPerFight != existedCfAgentBetSetting.LimitNumTicketPerFight)
            {
                auditBetSettings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightMainLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldMainLimitAmountPerFight,
                            NewValue = existedCfAgentBetSetting.MainLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightDrawLimitAmountPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldDrawLimitAmountPerFight,
                            NewValue = existedCfAgentBetSetting.DrawLimitAmountPerFight
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.CockFightLimitNumTicketPerFight,
                            BetKind = updatedCockFightBetKind?.Name,
                            OldValue = oldLimitNumTicketPerFight,
                            NewValue = existedCfAgentBetSetting.LimitNumTicketPerFight
                        }
                    });
            }
            await LotteryUow.SaveChangesAsync();

            if (auditBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = agent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = AuditDataHelper.GetAuditSupermasterId(agent),
                    MasterId = AuditDataHelper.GetAuditMasterId(agent),
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }
        }
    }
}
