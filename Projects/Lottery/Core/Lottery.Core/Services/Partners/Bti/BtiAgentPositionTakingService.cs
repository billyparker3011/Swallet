using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Services.Audit;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Bti;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lottery.Core.Helpers.RouteHelper.V1;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiAgentPositionTakingService : LotteryBaseService<BtiAgentPositionTakingService>, IBtiAgentPositionTakingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;

        public BtiAgentPositionTakingService(
            ILogger<BtiAgentPositionTakingService> logger,
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

        public async Task<BtiAgentPositionTakingModel> FindAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();
            var item = await repos.FindQueryBy(c => c.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return await PrepareModel(item);
        }

        public async Task<IEnumerable<BtiAgentPositionTakingModel>> GetsAsync(long agentId)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();
            var items = await repos.FindQueryBy(c => c.AgentId == agentId).ToListAsync();
            var results = new List<BtiAgentPositionTakingModel>();
            foreach (var item in items)
            {
                var result = await PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task<IEnumerable<BtiAgentPositionTakingModel>> GetAllAsync()
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();
            var items = await repos.FindQuery().ToListAsync();
            var results = new List<BtiAgentPositionTakingModel>();
            foreach (var item in items)
            {
                var result = await PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateAsync(BtiAgentPositionTakingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();
            var item = new BtiAgentPositionTaking()
            {
                AgentId = model.AgentId,
                BetKindId = model.BetKindId,
                PositionTaking = model.PositionTaking,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
            };

            await repos.AddAsync(item);

            //await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAsync(BtiAgentPositionTakingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();

            var item = await repos.FindByIdAsync(model.Id) ?? throw new NotFoundException();

            //var oldPT = item.PositionTaking;

            item.AgentId = model.AgentId;
            item.BetKindId = model.BetKindId;
            item.PositionTaking = model.PositionTaking;
            item.UpdatedAt = DateTime.Now;


            repos.Update(item);

            //await UpdateAudit(cAAgentPositionTaking, oldPT);

            //await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();

            repos.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task<List<BtiDefaultAgentPositionTakingModel>> GetDefaultPositionTaking(long agentId, int betKindId)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var repos = LotteryUow.GetRepository<IBtiAgentPositionTakingRepository>();

            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var defaultPositionTakings = new List<BtiAgentPositionTaking>();
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    defaultPositionTakings = await repos.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();
                    break;
                case (int)Role.Master:
                    defaultPositionTakings = await repos.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).ToListAsync();
                    break;
                case (int)Role.Agent:
                    defaultPositionTakings = await repos.FindQueryBy(c => betKindId > 0 ? c.BetKindId == betKindId : true).Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).ToListAsync();
                    break;
            }

            if (betKindId > 0 && !defaultPositionTakings.Any())
            {
                return new List<BtiDefaultAgentPositionTakingModel> { new BtiDefaultAgentPositionTakingModel(0, 1) };
            }
            return defaultPositionTakings.Select(x => new BtiDefaultAgentPositionTakingModel(x.BetKindId, x.PositionTaking)).ToList();
        }

        private async Task<BtiAgentPositionTakingModel> PrepareModel(BtiAgentPositionTaking item)
        {
            if (item == null) return new BtiAgentPositionTakingModel { DefaultPositionTaking = 1M };

            var defaultPositionTaking = await GetDefaultPositionTaking(item.AgentId, item.BetKindId);

            return new BtiAgentPositionTakingModel
            {
                Id = item.Id,
                AgentId = item.AgentId,
                BetKindId = item.BetKindId,
                BetKindName = item.BtiBetKind?.Name,
                DefaultPositionTaking = defaultPositionTaking.FirstOrDefault().DefaultPositionTaking,
                PositionTaking = item.PositionTaking
            };
        }
    }
}
