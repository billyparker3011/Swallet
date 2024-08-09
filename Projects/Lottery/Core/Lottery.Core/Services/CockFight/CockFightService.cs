using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetBalance;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightService : LotteryBaseService<CockFightService>, ICockFightService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CockFightService(
            ILogger<CockFightService> logger,
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

        public async Task CreateCockFightPlayer()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightPlayerBetSetting = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
            var accountId = cockFightRequestSetting.SettingValue?.PartnerAccountId ?? throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            // Create CockFight player
            bool isSuccessCreatedPlayer = true;
            var generatedMemberRefId = Guid.NewGuid().ToString();
            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial)
            {
                try
                {
                    await _partnerPublishService.Publish(new Ga28CreateMemberModel
                    {
                        Partner = PartnerType.GA28,
                        PlayerId = ClientContext.Player.PlayerId,
                        AccountId = accountId,
                        MemberRefId = generatedMemberRefId
                    });
                }
                catch (Exception ex)
                {
                    isSuccessCreatedPlayer = false;
                    Logger.LogError($"Create cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
                }

                if (!isSuccessCreatedPlayer) return;
                await cockFightPlayerMappingRepos.AddAsync(new Data.Entities.CockFightPlayerMapping
                {
                    PlayerId = ClientContext.Player.PlayerId,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId,
                    IsInitial = true,
                    IsEnabled = true,
                    IsFreeze = false,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0
                });
            }

            // Sync up player bet setting
            var playerBetSetting = await cockFightPlayerBetSetting.FindBetSettingByPlayerId(ClientContext.Player.PlayerId);
            try
            {
                await _partnerPublishService.Publish(new Ga28SyncUpBetSettingModel
                {
                    Partner = PartnerType.GA28,
                    MainLimitAmountPerFight = playerBetSetting?.MainLimitAmountPerFight,
                    DrawLimitAmountPerFight = playerBetSetting?.DrawLimitAmountPerFight,
                    LimitNumTicketPerFight = playerBetSetting?.LimitNumTicketPerFight,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Update bet setting cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
            }

            await LotteryUow.SaveChangesAsync();
        }

        public async Task<GetCockFightPlayerBalanceResult> GetCockFightPlayerBalance()
        {
            //TODO: Implement logic later
            return new GetCockFightPlayerBalanceResult { Balance = 9999.99m };
        }

        public async Task<LoginPlayerInformationDto> GetCockFightUrl()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial) return null;

            var clientUrlKey = ClientContext.Player.PlayerId.GetGa28ClientUrlByPlayerId();
            var clientUrlHash = await _redisCacheService.HashGetFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (!clientUrlHash.TryGetValue(clientUrlKey.SubKey, out string gameUrl)) return null;
            return new LoginPlayerInformationDto
            {
                GameClientUrl = gameUrl
            };
        }

        public async Task LoginCockFightPlayer()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
            var accountId = cockFightRequestSetting.SettingValue?.PartnerAccountId ?? throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial) throw new BadRequestException(ErrorCodeHelper.CockFight.PlayerHasNotBeenInitiatedYet);

            try
            {
                await _partnerPublishService.Publish(new Ga28LoginPlayerModel
                {
                    Partner = PartnerType.GA28,
                    AccountId = accountId,
                    MemberRefId = cockFightPlayerMapping.MemberRefId
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Login cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
            }
        }
    }
}
