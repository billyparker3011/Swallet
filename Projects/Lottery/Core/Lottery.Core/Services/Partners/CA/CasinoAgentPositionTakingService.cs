using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentPositionTakingService : LotteryBaseService<CasinoAgentPositionTakingService>, ICasinoAgentPositionTakingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;

        public CasinoAgentPositionTakingService(
            ILogger<CasinoAgentPositionTakingService> logger,
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

        public async Task<CasinoAgentPositionTakingModel> FindAgentPositionTakingAsync(long id)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            var item = await cAAgentPositionTakingRepository.FindQueryBy(c=>c.Id == id).Include(c=>c.CasinoBetKind).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return await PrepareModel(item);
        }

        public async Task<IEnumerable<CasinoAgentPositionTakingModel>> GetAgentPositionTakingsAsync(long agentId)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            var items =  await cAAgentPositionTakingRepository.FindQueryBy(c => c.AgentId == agentId).Include(c => c.CasinoBetKind).ToListAsync();
            var results = new List<CasinoAgentPositionTakingModel>();
            foreach (var item in items) 
            {
                var result = await PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task<IEnumerable<CasinoAgentPositionTakingModel>> GetAllAgentPositionTakingsAsync()
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            var items = await cAAgentPositionTakingRepository.FindQuery().Include(c => c.CasinoBetKind).ToListAsync();
            var results = new List<CasinoAgentPositionTakingModel>();
            foreach (var item in items)
            {
                var result = await PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();
            var cAAgentPositionTaking = new CasinoAgentPositionTaking()
            {
                AgentId = model.AgentId,
                BetKindId = model.BetKindId,
                PositionTaking = model.PositionTaking,
                CreatedAt = DateTime.Now,
            };

            await cAAgentPositionTakingRepository.AddAsync(cAAgentPositionTaking);

            await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            var cAAgentPositionTaking = await cAAgentPositionTakingRepository.FindByIdAsync(model.Id) ?? throw new NotFoundException();

            var oldPT = cAAgentPositionTaking.PositionTaking;

            cAAgentPositionTaking.AgentId = model.AgentId;
            cAAgentPositionTaking.BetKindId = model.BetKindId;
            cAAgentPositionTaking.PositionTaking = model.PositionTaking;
            cAAgentPositionTaking.UpdatedAt = DateTime.Now;


            cAAgentPositionTakingRepository.Update(cAAgentPositionTaking);

            await UpdateAudit(cAAgentPositionTaking, oldPT);

            await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAgentPositionTakingAsync(long id)
        {
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            cAAgentPositionTakingRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task<List<CasinoDefaultAgentPositionTakingModel>> GetDefaultPositionTaking(long agentId, int betKindId)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var cAAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var defaultPositionTakings = new List<CasinoAgentPositionTaking>();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultPositionTakings = await cAAgentPositionTakingRepository.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();
                    break;
                case (int)Role.Master:
                    defaultPositionTakings = await cAAgentPositionTakingRepository.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).ToListAsync();
                    break;
                case (int)Role.Agent:
                    defaultPositionTakings = await cAAgentPositionTakingRepository.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).ToListAsync();
                    break;
            }

            if(betKindId > 0 && !defaultPositionTakings.Any())
            {
               return new List<CasinoDefaultAgentPositionTakingModel> { new CasinoDefaultAgentPositionTakingModel(0, 1) };
            }
            return defaultPositionTakings.Select(x => new CasinoDefaultAgentPositionTakingModel(x.BetKindId, x.PositionTaking)).ToList();
        }

        private async Task<CasinoAgentPositionTakingModel> PrepareModel(CasinoAgentPositionTaking item)
        {
            if (item == null) return new CasinoAgentPositionTakingModel { DefaultPositionTaking = 1M};

            var defaultPositionTaking = await GetDefaultPositionTaking(item.AgentId, item.BetKindId);

            return new CasinoAgentPositionTakingModel
            {
                Id = item.Id,
                AgentId = item.AgentId,
                BetKindId = item.BetKindId,
                BetKindName = item.CasinoBetKind?.Name,
                DefaultPositionTaking = defaultPositionTaking.FirstOrDefault().DefaultPositionTaking,
                PositionTaking = item.PositionTaking
            };

        }

        private async Task UpdateChildPositionTaking(CasinoAgentPositionTaking item)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var casinoAgentPositionTakingRepository = LotteryUow.GetRepository<ICasinoAgentPositionTakingRepository>();

            var agent = await agentRepository.FindByIdAsync(item.AgentId) ?? throw new NotFoundException();
         
            var childAgentIds = await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId).Select(x => x.AgentId).ToListAsync();
            var existedChildCfAgentPositionTakings = await casinoAgentPositionTakingRepository.FindQueryBy(x => childAgentIds.Contains(x.AgentId) && x.BetKindId == item.BetKindId).ToListAsync();

            // Update all children of target agent if new value of agent is lower than the oldest one
            existedChildCfAgentPositionTakings.ForEach(childItem =>
            {
                childItem.PositionTaking = item.PositionTaking < childItem.PositionTaking ? item.PositionTaking : childItem.PositionTaking;
            });

        }

        private async Task UpdateAudit(CasinoAgentPositionTaking item, decimal oldPT)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var casinoBetKindRepository = LotteryUow.GetRepository<ICasinoBetKindRepository>();

            var agent = await agentRepository.FindByIdAsync(item.AgentId) ?? throw new NotFoundException();
            var betKind = await casinoBetKindRepository.FindByIdAsync(item.BetKindId) ?? throw new NotFoundException();

            var auditPositionTakings = new List<AuditSettingData>();

            auditPositionTakings.AddRange(new List<AuditSettingData>
            {
                new AuditSettingData
                {
                    Title = AuditDataHelper.Setting.PositionTakingTitle,
                    BetKind = betKind.Name,
                    OldValue = oldPT,
                    NewValue = item.PositionTaking
                }
            });

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
