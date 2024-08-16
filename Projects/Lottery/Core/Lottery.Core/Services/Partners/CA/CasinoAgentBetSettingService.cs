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
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var cAAgentBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingAgentHandicapRepository>();

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

            await cAAgentBetSettingRepository.AddAsync(cAAgentBetSetting);

            var cAAgentBetSettingAgentHandicaps = new List<CasinoAgentBetSettingAgentHandicap>();

            cAAgentBetSettingAgentHandicaps.AddRange(model.DefaultGeneralHandicapIds.Select(c =>          
                new CasinoAgentBetSettingAgentHandicap()
                {
                    CasinoAgentBetSetting = cAAgentBetSetting,
                    CasinoAgentHandicapId = c
                }
            ).ToList());


            await cAAgentBetSettingAgentHandicapRepository.AddRangeAsync(cAAgentBetSettingAgentHandicaps);

            await LotteryUow.SaveChangesAsync();
        }

        public async Task UpdateAgentBetSettingAsync(UpdateCasinoAgentBetSettingModel model)
        {
            var cAAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var cAAgentBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingAgentHandicapRepository>();

            var cAAgentBetSetting = await cAAgentBetSettingRepository.FindByIdAsync(model.Id);

            cAAgentBetSetting.BetKindId = model.BetKindId;
            cAAgentBetSetting.DefaultVipHandicapId = model.DefaultVipHandicapId;
            cAAgentBetSetting.MinBet = model.MinBet;
            cAAgentBetSetting.MaxBet = model.MinBet;
            cAAgentBetSetting.MaxWin = model.MaxWin;
            cAAgentBetSetting.MaxLose = model.MaxLose;
            cAAgentBetSetting.UpdatedAt = DateTime.Now;


            cAAgentBetSettingRepository.Update(cAAgentBetSetting);

            var cAAgentBetSettingAgentHandicapsOld = await cAAgentBetSettingAgentHandicapRepository.FindByAsync(c => c.CasinoAgentBetSettingId == model.Id);

            cAAgentBetSettingAgentHandicapRepository.DeleteItems(cAAgentBetSettingAgentHandicapsOld);

            var cAAgentBetSettingAgentHandicapsNew = new List<CasinoAgentBetSettingAgentHandicap>();

            cAAgentBetSettingAgentHandicapsNew.AddRange(model.DefaultGeneralHandicapIds.Select(c =>
                new CasinoAgentBetSettingAgentHandicap()
                {
                    CasinoAgentBetSetting = cAAgentBetSetting,
                    CasinoAgentHandicapId = c
                }
            ).ToList());

            await cAAgentBetSettingAgentHandicapRepository.AddRangeAsync(cAAgentBetSettingAgentHandicapsNew);

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

    }
}
