using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Services.Audit;
using Lottery.Core.Services.Partners.CA;
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

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiAgentBetSettingService : LotteryBaseService<BtiAgentBetSettingService>, IBtiAgentBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAuditService _auditService;

        public BtiAgentBetSettingService(
            ILogger<BtiAgentBetSettingService> logger,
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

        public async Task<BtiAgentBetSettingModel> FindAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var item = await repos.FindQueryBy(c => c.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return PrepareModel(item);
        }

        public async Task<IEnumerable<BtiAgentBetSettingModel>> GetsAsync(long agentId)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var items = await repos.FindQueryBy(c => c.AgentId == agentId).ToListAsync();
            var results = new List<BtiAgentBetSettingModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task<IEnumerable<BtiAgentBetSettingModel>> GetAllAsync()
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var items = await repos.FindQuery().ToListAsync();
            var results = new List<BtiAgentBetSettingModel>();
            foreach (var item in items)
            {
                var result = PrepareModel(item);
                results.Add(result);
            }
            return results;
        }

        public async Task CreateAsync(BtiAgentBetSettingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();
            var item = new BtiAgentBetSetting()
            {
                AgentId = model.AgentId,
                BetKindId = model.BetKindId,
                MinBet = model.MinBet,
                MaxBet = model.MaxBet,
                MaxWin = model.MaxWin,
                MaxLoss = model.MaxLoss,
                CreatedAt = DateTime.Now,
                CreatedBy = 0,
            };

            await repos.AddAsync(item);

            //await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();

        }

        public async Task UpdateAsync(BtiAgentBetSettingModel model)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();

            var item = await repos.FindByIdAsync(model.Id) ?? throw new NotFoundException();

            //var oldPT = item.PositionTaking;

            item.AgentId = model.AgentId;
            item.BetKindId = model.BetKindId;
            item.MinBet = model.MinBet;
            item.MaxBet = model.MaxBet;
            item.MaxWin = model.MaxWin;
            item.MaxLoss = model.MaxLoss;
            item.UpdatedAt = DateTime.Now;


            repos.Update(item);

            //await UpdateAudit(cAAgentPositionTaking, oldPT);

            //await UpdateChildPositionTaking(cAAgentPositionTaking);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var repos = LotteryUow.GetRepository<IBtiAgentBetSettingRepository>();

            repos.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        private BtiAgentBetSettingModel PrepareModel(BtiAgentBetSetting item)
        {
            return new BtiAgentBetSettingModel()
            {
                Id = item.Id,
                AgentId = item.AgentId,
                BetKindId = item.BetKindId,
                MinBet = item.MinBet,
                MaxBet = item.MaxBet,
                MaxWin = item.MaxWin,
                MaxLoss = item.MaxLoss,

            };
        }
    }
}
