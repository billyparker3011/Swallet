using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Enums;
using HnMicro.Core.Helpers;
using Lottery.Core.Repositories.Player;
using HnMicro.Framework.Exceptions;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentBetSettingService : LotteryBaseService<CasinoAgentBetSettingService>, ICasinoAgentBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoAgentBetSettingService(
            ILogger<CasinoAgentBetSettingService> logger,
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

        public async Task<CasinoAgentBetSetting> FindAgentBetSettingAsync(long id)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            return await cAAgentBetSettingRepository.FindByIdAsync(id);
        }

        public async Task<CasinoAgentBetSetting> FindAgentBetSettingWithIncludeAsync(long id)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            return await cAAgentBetSettingRepository.FindQueryBy(c => c.Id == id).Include(c => c.CasinoAgentBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.DefaultVipHandicap).Include(c => c.Agent).Include(c => c.BetKind).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CasinoAgentBetSetting>> GetAgentBetSettingsAsync(long agentId)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            return await cAAgentBetSettingRepository.FindByAsync(c => c.AgentId == agentId);
        }

        public async Task<IEnumerable<CasinoAgentBetSetting>> GetAgentBetSettingsWithIncludeAsync(long agentId)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            return await cAAgentBetSettingRepository.FindQueryBy(c => c.AgentId == agentId).Include(c => c.CasinoAgentBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.DefaultVipHandicap).Include(c => c.Agent).Include(c => c.BetKind).ToListAsync();
        }

        public async Task<IEnumerable<CasinoAgentBetSetting>> GetAllAgentBetSettingsAsync()
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            return cAAgentBetSettingRepository.GetAll();
        }

        public async Task CreateAgentBetSettingAsync(CreateCasinoAgentBetSettingModel model)
        {
           
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var agentHandicapRepos = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            var agentBetSettingRepos = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var playerBetSettingRepos = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var agentBetSettingAgentHandicapRepos = LotteryUow.GetRepository<ICasinoAgentBetSettingAgentHandicapRepository>();
            var playerBetSettingAgentHandicapRepos = LotteryUow.GetRepository<ICasinoPlayerBetSettingAgentHandicapRepository>();

            var agent = await agentRepos.FindByIdAsync(model.AgentId) ?? throw new NotFoundException();

            var cAAgentBetSetting = new CasinoAgentBetSetting()
            {
                AgentId = model.AgentId,
                BetKindId = model.BetKindId,
                DefaultVipHandicapId = model.DefaultVipHandicapId,
                MinBet = model.MinBet,
                MaxBet = model.MinBet,
                MaxWin = model.MaxWin,
                MaxLose = model.MaxLose,
                CreatedAt = DateTime.Now,
                CreatedBy = model.AgentId,
            };

            await agentBetSettingRepos.AddAsync(cAAgentBetSetting);

            var cAAgentBetSettingAgentHandicaps = new List<CasinoAgentBetSettingAgentHandicap>();

            cAAgentBetSettingAgentHandicaps.AddRange(model.DefaultGeneralHandicapIds.Select(c =>          
                new CasinoAgentBetSettingAgentHandicap()
                {
                    CasinoAgentBetSetting = cAAgentBetSetting,
                    CasinoAgentHandicapId = c
                }
            ).ToList());


            await agentBetSettingAgentHandicapRepos.AddRangeAsync(cAAgentBetSettingAgentHandicaps);

            // Update child
            await UpdateChildAgentBetSetting(model.DefaultGeneralHandicapIds, model.DefaultVipHandicapId, agent, agentBetSettingRepos, agentBetSettingAgentHandicapRepos, agentHandicapRepos, agentRepos);
            await UpdateChildPlayerBetSetting(model.DefaultGeneralHandicapIds, model.DefaultVipHandicapId, agent, playerBetSettingRepos, playerBetSettingAgentHandicapRepos, agentHandicapRepos, playerRepos);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task UpdateAgentBetSettingAsync(UpdateCasinoAgentBetSettingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var agentHandicapRepos = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            var agentBetSettingRepos = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var playerBetSettingRepos = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var agentBetSettingAgentHandicapRepos = LotteryUow.GetRepository<ICasinoAgentBetSettingAgentHandicapRepository>();
            var playerBetSettingAgentHandicapRepos = LotteryUow.GetRepository<ICasinoPlayerBetSettingAgentHandicapRepository>();

            var cAAgentBetSetting = await agentBetSettingRepos.FindByIdAsync(model.Id) ?? throw new NotFoundException();

            var agent = await agentRepos.FindByIdAsync(cAAgentBetSetting.AgentId) ?? throw new NotFoundException();

           

            cAAgentBetSetting.BetKindId = model.BetKindId;
            cAAgentBetSetting.DefaultVipHandicapId = model.DefaultVipHandicapId;
            cAAgentBetSetting.MinBet = model.MinBet;
            cAAgentBetSetting.MaxBet = model.MinBet;
            cAAgentBetSetting.MaxWin = model.MaxWin;
            cAAgentBetSetting.MaxLose = model.MaxLose;
            cAAgentBetSetting.UpdatedAt = DateTime.Now;


            agentBetSettingRepos.Update(cAAgentBetSetting);

            var cAAgentBetSettingAgentHandicapsOld = await agentBetSettingAgentHandicapRepos.FindByAsync(c => c.CasinoAgentBetSettingId == model.Id);

            agentBetSettingAgentHandicapRepos.DeleteItems(cAAgentBetSettingAgentHandicapsOld);

            var cAAgentBetSettingAgentHandicapsNew = new List<CasinoAgentBetSettingAgentHandicap>();

            cAAgentBetSettingAgentHandicapsNew.AddRange(model.DefaultGeneralHandicapIds.Select(c =>
                new CasinoAgentBetSettingAgentHandicap()
                {
                    CasinoAgentBetSetting = cAAgentBetSetting,
                    CasinoAgentHandicapId = c
                }
            ).ToList());

            await agentBetSettingAgentHandicapRepos.AddRangeAsync(cAAgentBetSettingAgentHandicapsNew);

            // Update child
            await UpdateChildAgentBetSetting(model.DefaultGeneralHandicapIds, model.DefaultVipHandicapId, agent, agentBetSettingRepos, agentBetSettingAgentHandicapRepos, agentHandicapRepos, agentRepos);
            await UpdateChildPlayerBetSetting(model.DefaultGeneralHandicapIds, model.DefaultVipHandicapId, agent, playerBetSettingRepos, playerBetSettingAgentHandicapRepos, agentHandicapRepos, playerRepos);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task DeleteAgentBetSettingAsync(long id)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var cAAgentBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingAgentHandicapRepository>();
            var cAAgentBetSettingAgentHandicaps = await cAAgentBetSettingAgentHandicapRepository.FindByAsync(c => c.CasinoAgentBetSettingId == id);

            cAAgentBetSettingAgentHandicapRepository.DeleteItems(cAAgentBetSettingAgentHandicaps);

            cAAgentBetSettingRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();
        }

        public string[] GetStringGeneralHandicaps(CasinoAgentBetSetting item)
        {
            if (item == null || !item.CasinoAgentBetSettingAgentHandicaps.Any()) return null;
            return item.CasinoAgentBetSettingAgentHandicaps.Select(c => c.CasinoAgentHandicap.Name).ToArray();
        }

        private async Task UpdateChildAgentBetSetting(List<int> generalHandicapIds, int vipHandicapId, Data.Entities.Agent agent, ICasinoAgentBetSettingRepository casinoAgentBetSettingRepos, ICasinoAgentBetSettingAgentHandicapRepository casinoAgentBetSettingAgentHandicapRepos, ICasinoAgentHandicapRepository agentHandicapRepos, IAgentRepository agentRepos)
        {
            var queryGeneral = await agentHandicapRepos.FindQueryBy(c=> generalHandicapIds.Contains(c.Id)).ToListAsync();
            var queryVip = await agentHandicapRepos.FindByIdAsync(vipHandicapId);

            var maxGeneral = queryGeneral.FirstOrDefault(c=>c.MaxBet == queryGeneral.Max(c => c.MaxBet));
            var minGeneral = queryGeneral.FirstOrDefault(c => c.MinBet == queryGeneral.Min(c => c.MinBet));

            var childAgentIds = await GetChildAgentIds(agentRepos, agent);
            var childAgentBettings = await casinoAgentBetSettingRepos.FindQueryBy(c => childAgentIds.Contains(c.AgentId) && (c.CasinoAgentBetSettingAgentHandicaps.Max(c => c.CasinoAgentHandicap.MaxBet) > maxGeneral.MaxBet 
            || c.CasinoAgentBetSettingAgentHandicaps.Min(c => c.CasinoAgentHandicap.MinBet) < minGeneral.MinBet 
            || c.DefaultVipHandicap.MaxBet > queryVip.MaxBet
            || c.DefaultVipHandicap.MinBet > queryVip.MinBet))
                .Include(c=>c.CasinoAgentBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c=>c.DefaultVipHandicap).ToListAsync();

            foreach(var agentBetSetting in childAgentBettings)
            {
                var agentHandicapMaxs = agentBetSetting.CasinoAgentBetSettingAgentHandicaps.Where(c => c.CasinoAgentHandicap.MaxBet > maxGeneral.MaxBet).ToList();
                var agentHandicapMins = agentBetSetting.CasinoAgentBetSettingAgentHandicaps.Where(c => c.CasinoAgentHandicap.MinBet < minGeneral.MinBet).ToList();

                var agentHandicapsDelete = agentHandicapMaxs.Concat(agentHandicapMins).Distinct().ToList();

                foreach (var agentHandicap in agentHandicapsDelete)
                {
                    casinoAgentBetSettingAgentHandicapRepos.Delete(agentHandicap);
                }

                if(agentHandicapMaxs.Any())
                    await casinoAgentBetSettingAgentHandicapRepos.AddAsync(new CasinoAgentBetSettingAgentHandicap()
                    {
                        CasinoAgentBetSettingId = agentHandicapMaxs.FirstOrDefault().CasinoAgentBetSettingId,
                        CasinoAgentHandicapId = maxGeneral.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0
                    });

                if (agentHandicapMins.Any() && maxGeneral.Id != minGeneral.Id)
                    await casinoAgentBetSettingAgentHandicapRepos.AddAsync(new CasinoAgentBetSettingAgentHandicap()
                    {
                        CasinoAgentBetSettingId = agentHandicapMaxs.FirstOrDefault().CasinoAgentBetSettingId,
                        CasinoAgentHandicapId = minGeneral.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0
                    });

                if (agentBetSetting.DefaultVipHandicap.MaxBet > queryVip.MaxBet)
                {
                    agentBetSetting.DefaultVipHandicapId = queryVip.Id;
                    casinoAgentBetSettingRepos.Update(agentBetSetting);
                }
                if (agentBetSetting.DefaultVipHandicap.MinBet < queryVip.MinBet)
                {
                    agentBetSetting.DefaultVipHandicapId = queryVip.Id;
                    casinoAgentBetSettingRepos.Update(agentBetSetting);
                }
            }

        }

        private async Task UpdateChildPlayerBetSetting(List<int> generalHandicapIds, int vipHandicapId, Data.Entities.Agent agent, ICasinoPlayerBetSettingRepository casinoPlayerBetSettingRepos, ICasinoPlayerBetSettingAgentHandicapRepository casinoPlayerBetSettingAgentHandicapRepos, ICasinoAgentHandicapRepository agentHandicapRepos, IPlayerRepository playerRepos)
        {
            var queryGeneral = await agentHandicapRepos.FindQueryBy(c => generalHandicapIds.Contains(c.Id)).ToListAsync();
            var queryVip = await agentHandicapRepos.FindByIdAsync(vipHandicapId);

            var maxGeneral = queryGeneral.FirstOrDefault(c => c.MaxBet == queryGeneral.Max(c => c.MaxBet));
            var minGeneral = queryGeneral.FirstOrDefault(c => c.MinBet == queryGeneral.Min(c => c.MinBet));

            var childPlayerIds = await GetChildPlayerIds(playerRepos, agent);
            var childPlayerBettings = await casinoPlayerBetSettingRepos.FindQueryBy(c => childPlayerIds.Contains(c.PlayerId) && (c.CasinoPlayerBetSettingAgentHandicaps.Max(c => c.CasinoAgentHandicap.MaxBet) > maxGeneral.MaxBet
            || c.CasinoPlayerBetSettingAgentHandicaps.Min(c => c.CasinoAgentHandicap.MinBet) < minGeneral.MinBet
            || c.CasinoAgentHandicap.MaxBet > queryVip.MaxBet
            || c.CasinoAgentHandicap.MinBet > queryVip.MinBet))
                .Include(c => c.CasinoPlayerBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.CasinoAgentHandicap).ToListAsync();

            foreach (var agentBetSetting in childPlayerBettings)
            {
                var playerHandicapMaxs = agentBetSetting.CasinoPlayerBetSettingAgentHandicaps.Where(c => c.CasinoAgentHandicap.MaxBet > maxGeneral.MaxBet).ToList();
                var playerHandicapMins = agentBetSetting.CasinoPlayerBetSettingAgentHandicaps.Where(c => c.CasinoAgentHandicap.MinBet < minGeneral.MinBet).ToList();

                var playerHandicapsDelete = playerHandicapMaxs.Concat(playerHandicapMins).Distinct().ToList();

                foreach (var playerHandicap in playerHandicapsDelete)
                {
                    casinoPlayerBetSettingAgentHandicapRepos.Delete(playerHandicap);
                }

                if (playerHandicapMaxs.Any())
                    await casinoPlayerBetSettingAgentHandicapRepos.AddAsync(new CasinoPlayerBetSettingAgentHandicap()
                    {
                        CasinoPlayerBetSettingId = playerHandicapMaxs.FirstOrDefault().CasinoPlayerBetSettingId,
                        CasinoAgentHandicapId = maxGeneral.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0
                    });

                if (playerHandicapMins.Any() && maxGeneral.Id != minGeneral.Id)
                    await casinoPlayerBetSettingAgentHandicapRepos.AddAsync(new CasinoPlayerBetSettingAgentHandicap()
                    {
                        CasinoPlayerBetSettingId = playerHandicapMaxs.FirstOrDefault().CasinoPlayerBetSettingId,
                        CasinoAgentHandicapId = minGeneral.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(8),
                        CreatedBy = 0
                    });

                if (agentBetSetting.CasinoAgentHandicap.MaxBet > queryVip.MaxBet)
                {
                    agentBetSetting.VipHandicapId = queryVip.Id;
                    casinoPlayerBetSettingRepos.Update(agentBetSetting);
                }
                if (agentBetSetting.CasinoAgentHandicap.MinBet < queryVip.MinBet)
                {
                    agentBetSetting.VipHandicapId = queryVip.Id;
                    casinoPlayerBetSettingRepos.Update(agentBetSetting);
                }
            }

        }

        private async Task<List<long>> GetChildAgentIds(IAgentRepository agentRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Company:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync(); 
                case (int)Role.Supermaster:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.SupermasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                case (int)Role.Master:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.MasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                case (int)Role.Agent:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.AgentId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                default:
                    return new List<long>();
            }
        }

        private async Task<List<long>> GetChildPlayerIds(IPlayerRepository playerRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Company:
                    return await playerRepository.FindQuery().Select(x => x.PlayerId).ToListAsync();
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

    }
}
