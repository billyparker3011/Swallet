using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetCockFightAgentPositionTaking;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentPositionTaking;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentPositionTakingService : LotteryBaseService<CockFightAgentPositionTakingService>, ICockFightAgentPositionTakingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;
        public CockFightAgentPositionTakingService(
            ILogger<CockFightAgentPositionTakingService> logger,
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

        public async Task<GetCockFightAgentPositionTakingResult> GetCockFightAgentPositionTakingDetail(long agentId)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cockFightAgentPositionTakingRepos = LotteryUow.GetRepository<ICockFightAgentPositionTakingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var defaultPositionTaking = new Data.Entities.Partners.CockFight.CockFightAgentPostionTaking();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultPositionTaking = await cockFightAgentPositionTakingRepos.FindQuery().Include(x => x.Agent).FirstOrDefaultAsync(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L);
                    break;
                case (int)Role.Master:
                    defaultPositionTaking = await cockFightAgentPositionTakingRepos.FindQuery().Include(x => x.Agent).FirstOrDefaultAsync(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId);
                    break;
                case (int)Role.Agent:
                    defaultPositionTaking = await cockFightAgentPositionTakingRepos.FindQuery().Include(x => x.Agent).FirstOrDefaultAsync(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId);
                    break;
            }

            return await cockFightAgentPositionTakingRepos.FindQuery()
                                                .Include(x => x.Agent)
                                                .Include(x => x.CockFightBetKind)
                                                .Where(x => x.Agent.AgentId == targetAgent.AgentId
                                                            && x.Agent.RoleId == targetAgent.RoleId)
                                                .Select(x => new GetCockFightAgentPositionTakingResult
                                                {
                                                    BetKindId = x.CockFightBetKind.Id,
                                                    BetKindName = x.CockFightBetKind.Name,
                                                    DefaultPositionTaking = defaultPositionTaking != null ? defaultPositionTaking.PositionTaking : x.PositionTaking,
                                                    ActualPositionTaking = x.PositionTaking
                                                })
                                                .FirstOrDefaultAsync();
        }

        public async Task UpdateCockFightAgentPositionTaking(long agentId, UpdateCockFightAgentPositionTakingModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var cockFightAgentPositionTakingRepository = LotteryUow.GetRepository<ICockFightAgentPositionTakingRepository>();
            var cockFightBetKindRepos = LotteryUow.GetRepository<ICockFightBetKindRepository>();

            var agent = await agentRepository.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var auditPositionTakings = new List<AuditSettingData>();
            var updatedBetKind = await cockFightBetKindRepos.FindByIdAsync(model.BetKindId) ?? throw new NotFoundException();
            var existedCfAgentPositionTaking = await cockFightAgentPositionTakingRepository.FindQueryBy(x => x.AgentId == agent.AgentId && x.BetKindId == updatedBetKind.Id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var childAgentIds = await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId).Select(x => x.AgentId).ToListAsync();
            var existedChildCfAgentPositionTakings = await cockFightAgentPositionTakingRepository.FindQueryBy(x => childAgentIds.Contains(x.AgentId) && x.BetKindId == updatedBetKind.Id).ToListAsync();

            // Update agent PT
            var oldPTValue = existedCfAgentPositionTaking.PositionTaking;
            existedCfAgentPositionTaking.PositionTaking = model.ActualPositionTaking;

            // Update all children of target agent if new value of agent is lower than the oldest one
            existedChildCfAgentPositionTakings.ForEach(childItem =>
            {
                childItem.PositionTaking = existedCfAgentPositionTaking.PositionTaking < childItem.PositionTaking ? existedCfAgentPositionTaking.PositionTaking : childItem.PositionTaking;
            });

            auditPositionTakings.AddRange(new List<AuditSettingData>
            {
                new AuditSettingData
                {
                    Title = AuditDataHelper.Setting.PositionTakingTitle,
                    BetKind = updatedBetKind.Name,
                    OldValue = oldPTValue,
                    NewValue = existedCfAgentPositionTaking.PositionTaking
                }
            });
            await LotteryUow.SaveChangesAsync();

            if (auditPositionTakings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = agent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdatePositionTaking,
                    SupermasterId = AuditDataHelper.GetAuditSupermasterId(agent),
                    MasterId = AuditDataHelper.GetAuditMasterId(agent),
                    AuditSettingDatas = auditPositionTakings.OrderBy(x => x.BetKind).ToList()
                });
            }
        }
    }
}
