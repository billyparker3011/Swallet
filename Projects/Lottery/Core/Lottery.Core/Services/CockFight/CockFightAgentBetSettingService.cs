using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentBetSettingService : LotteryBaseService<CockFightAgentBetSettingService>, ICockFightAgentBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        public CockFightAgentBetSettingService(
            ILogger<CockFightAgentBetSettingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
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
            throw new NotImplementedException();
        }
    }
}
