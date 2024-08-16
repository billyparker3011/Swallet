using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoPlayerBetSettingService : LotteryBaseService<CasinoPlayerBetSettingService>, ICasinoPlayerBetSettingService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoPlayerBetSettingService(
            ILogger<CasinoPlayerBetSettingService> logger,
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

        public async Task<CasinoPlayerBetSetting> FindPlayerBetSettingAsync(long id)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            return await cAPlayerBetSettingRepository.FindByIdAsync(id);
        }

        public async Task<IEnumerable<CasinoPlayerBetSetting>> GetPlayerBetSettingsAsync(long playerId)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            return await cAPlayerBetSettingRepository.FindByAsync(c => c.PlayerId == playerId);
        }

        public async Task<IEnumerable<CasinoPlayerBetSetting>> GetPlayerBetSettingsWithIncludeAsync(long playerId)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            return await cAPlayerBetSettingRepository.FindQueryBy(c => c.PlayerId == playerId).Include(c=>c.CasinoPlayerBetSettingAgentHandicaps).ThenInclude(c=>c.CasinoAgentHandicap).Include(c => c.CasinoAgentHandicap).Include(c=>c.Player).Include(c=>c.CasinoBetKind).ToListAsync();
        }

        public async Task<IEnumerable<CasinoPlayerBetSetting>> GetAllPlayerBetSettingsAsync()
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            return cAPlayerBetSettingRepository.GetAll();
        }

        public async Task CreatePlayerBetSettingAsync(CreateCasinoPlayerBetSettingModel model)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var cAPlayerBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingAgentHandicapRepository>();

            var cAPlayerBetSetting = new CasinoPlayerBetSetting()
            {
                PlayerId = model.PlayerId,
                BetKindId = model.BetKindId,
                VipHandicapId = model.VipHandicapId,
                MinBet = model.MinBet,
                MaxBet = model.MinBet,
                MaxWin = model.MaxWin,
                MaxLose = model.MaxLose,
                CreatedAt = DateTime.Now,
            };

            await cAPlayerBetSettingRepository.AddAsync(cAPlayerBetSetting);

            var cAPlayerBetSettingAgentHandicaps = new List<CasinoPlayerBetSettingAgentHandicap>();

            cAPlayerBetSettingAgentHandicaps.AddRange(model.GeneralHandicapIds.Select(c =>      
                new CasinoPlayerBetSettingAgentHandicap()
                {
                    CasinoPlayerBetSetting = cAPlayerBetSetting,
                    CasinoAgentHandicapId = c
                }).ToList());


            await cAPlayerBetSettingAgentHandicapRepository.AddRangeAsync(cAPlayerBetSettingAgentHandicaps);

            var cAPlayerMapping = await cAPlayerMappingRepository.FindQueryBy(c=>c.PlayerId == model.PlayerId).FirstOrDefaultAsync();
            if(cAPlayerMapping != null)
            {
                cAPlayerMapping.IsAccountEnable = model.IsAccountEnable;
                cAPlayerMapping.IsAlowedToBet = model.IsAllowedToBet;
                cAPlayerMappingRepository.Update(cAPlayerMapping);
            }

            await LotteryUow.SaveChangesAsync();

            await _partnerPublishService.Publish(new CasinoAllBetPlayerBetSettingModel
            {
                Partner = PartnerType.Allbet,
                PlayerId = cAPlayerBetSetting.PlayerId,
                GeneralHandicapIds = model.GeneralHandicapIds,
                VipHandicapId = model.VipHandicapId,
                IsAllowedToBet = model.IsAllowedToBet,
                IsAccountEnabled = model.IsAccountEnable
            });

        }

        public async Task UpdatePlayerBetSettingAsync(UpdateCasinoPlayerBetSettingModel model)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var cAPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var cAPlayerBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingAgentHandicapRepository>();


            var cAPlayerBetSetting = await cAPlayerBetSettingRepository.FindByIdAsync(model.Id);

            cAPlayerBetSetting.PlayerId = model.PlayerId;
            cAPlayerBetSetting.BetKindId = model.BetKindId;
            cAPlayerBetSetting.VipHandicapId = model.VipHandicapId;
            cAPlayerBetSetting.MinBet = model.MinBet;
            cAPlayerBetSetting.MaxBet = model.MinBet;
            cAPlayerBetSetting.MaxWin = model.MaxWin;
            cAPlayerBetSetting.MaxLose = model.MaxLose;
            cAPlayerBetSetting.UpdatedAt = DateTime.Now;


            cAPlayerBetSettingRepository.Update(cAPlayerBetSetting);

            var cAPlayerBetSettingAgentHandicapsOld = await cAPlayerBetSettingAgentHandicapRepository.FindByAsync(c => c.CasinoPlayerBetSettingId == model.Id);

            cAPlayerBetSettingAgentHandicapRepository.DeleteItems(cAPlayerBetSettingAgentHandicapsOld);

            var cAPlayerBetSettingAgentHandicapsNew = new List<CasinoPlayerBetSettingAgentHandicap>();

            cAPlayerBetSettingAgentHandicapsNew.AddRange(model.GeneralHandicapIds.Select(c =>
                new CasinoPlayerBetSettingAgentHandicap()
                {
                    CasinoPlayerBetSetting = cAPlayerBetSetting,
                    CasinoAgentHandicapId = c
                }
            ).ToList());

            await cAPlayerBetSettingAgentHandicapRepository.AddRangeAsync(cAPlayerBetSettingAgentHandicapsNew);

            var cAPlayerMapping = await cAPlayerMappingRepository.FindQueryBy(c=>c.PlayerId == model.PlayerId).FirstOrDefaultAsync();
            if (cAPlayerMapping != null)
            {
                cAPlayerMapping.IsAccountEnable = model.IsAccountEnabled;
                cAPlayerMapping.IsAlowedToBet = model.IsAllowedToBet;
                cAPlayerMappingRepository.Update(cAPlayerMapping);
            }

            await LotteryUow.SaveChangesAsync();

            await _partnerPublishService.Publish(new CasinoAllBetPlayerBetSettingModel
            {
                Partner = PartnerType.Allbet,
                PlayerId = cAPlayerBetSetting.PlayerId,
                GeneralHandicapIds = model.GeneralHandicapIds,
                VipHandicapId = model.VipHandicapId,
                IsAllowedToBet = model.IsAllowedToBet,
                IsAccountEnabled = model.IsAccountEnabled
            });

        }

        public async Task DeletePlayerBetSettingAsync(long id)
        {
            var cAPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var cAPlayerBetSettingAgentHandicapRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingAgentHandicapRepository>();
            var cAAgentBetSettingAgentHandicaps = await cAPlayerBetSettingAgentHandicapRepository.FindByAsync(c => c.CasinoPlayerBetSettingId == id);

            cAPlayerBetSettingAgentHandicapRepository.DeleteItems(cAAgentBetSettingAgentHandicaps);

            cAPlayerBetSettingRepository.DeleteById(id);

            await LotteryUow.SaveChangesAsync();

        }

        public string[] GetStringGeneralHandicaps(CasinoPlayerBetSetting item)
        {
            if (item == null || !item.CasinoPlayerBetSettingAgentHandicaps.Any()) return null;
            return item.CasinoPlayerBetSettingAgentHandicaps.Select(c=>c.CasinoAgentHandicap.Name).ToArray();
        }
    }
}
