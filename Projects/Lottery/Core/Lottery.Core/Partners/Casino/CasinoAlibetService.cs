using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Partners.CA;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Partners.Casino
{
    public class CasinoAlibetService : BasePartnerType
    {
        private readonly ICasinoService _cAService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoAlibetService(ILogger<BasePartnerType> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration, 
            IClockService clockService, 
            IHttpClientFactory httpClientFactory, 
            ILotteryUow lotteryUow,
            ICasinoService cAService,
            IRedisCacheService redisCacheService
            ) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow)
        {
            _cAService = cAService;
            _redisCacheService = redisCacheService;

        }

        public override PartnerType PartnerType { get; set; } = PartnerType.Allbet;

        public override async Task CreatePlayer(object data)
        {
            if (data is null) return;

            var createPartnerPlayerModel = data as CasinoAllBetPlayerModel;

            var _casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var _casinoPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var _casinoAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();

            //Check player is synced
            var cAPlayerMapping = await _casinoPlayerMappingRepository.FindQueryBy(c=>c.PlayerId == createPartnerPlayerModel.PlayerId).FirstOrDefaultAsync();

            if (cAPlayerMapping != null) return;

            //Create new player
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(createPartnerPlayerModel.PlayerId) ?? throw new NotFoundException();
            var partnerSetting = await _cAService.GetCasinoBookieSettingValueAsync() ?? throw new NotFoundException();
            var partnerUsername = _cAService.GeneralUsername(player.PlayerId.ToString(), partnerSetting.Suffix).ToLowerInvariant();

            //Set username and agent
            createPartnerPlayerModel.Player = partnerUsername;
            createPartnerPlayerModel.Agent = partnerSetting.Agent;

            //Check or Create in partner
            var resultCheckOrCreate = await _cAService.SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.CheckOrCreate, createPartnerPlayerModel.ToBodyJson());

            //Success
            if (resultCheckOrCreate.IsSuccessStatusCode)
            {
                //Update player bet setting in partner
                var playerBetSetting = new CasinoAllBetPlayerBetSettingModel()
                {
                    Player = partnerUsername,
                    Nickname = createPartnerPlayerModel.NickName,
                    IsAccountEnabled = createPartnerPlayerModel.IsAccountEnabled,
                    IsAllowedToBet = createPartnerPlayerModel.IsAlowedToBet,
                };
                //Get bet setting
                var caPlayerBetSettings = await _casinoPlayerBetSettingRepository.FindQueryBy(c => c.PlayerId == createPartnerPlayerModel.PlayerId).Include(c => c.CasinoPlayerBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.CasinoAgentHandicap).Include(c => c.Player).Include(c => c.CasinoBetKind).ToListAsync();

                if (caPlayerBetSettings != null && caPlayerBetSettings.Any())
                {
                    var caPlayerBetSetting = caPlayerBetSettings.FirstOrDefault();
                    playerBetSetting.GeneralHandicaps = GetStringGeneralHandicaps(caPlayerBetSetting.CasinoPlayerBetSettingAgentHandicaps.Select(c=>c.CasinoAgentHandicap));
                    playerBetSetting.VipHandicap = caPlayerBetSetting.CasinoAgentHandicap.Name;
                }
                else
                {
                    var caAgentBetSettings = await _casinoAgentBetSettingRepository.FindQueryBy(c => c.AgentId == player.AgentId).Include(c => c.CasinoAgentBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.DefaultVipHandicap).Include(c => c.Agent).Include(c => c.BetKind).ToListAsync();
                    if (caAgentBetSettings != null && caAgentBetSettings.Any())
                    {
                        var caAgentBetSetting = caAgentBetSettings.FirstOrDefault();
                        playerBetSetting.GeneralHandicaps = GetStringGeneralHandicaps(caAgentBetSetting.CasinoAgentBetSettingAgentHandicaps.Select(c => c.CasinoAgentHandicap));
                        playerBetSetting.VipHandicap = caAgentBetSetting.DefaultVipHandicap.Name;
                    }
                }

                var resultModifyPlayerSetting = await _cAService.SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson());

                //Create Player Mapping
                var createPlayerMappingModel = new CreateCasinoPlayerMappingModel()
                {
                    PlayerId = createPartnerPlayerModel.PlayerId,
                    NickName = createPartnerPlayerModel.NickName,
                    IsAccountEnable = createPartnerPlayerModel.IsAccountEnabled,
                    IsAlowedToBet = createPartnerPlayerModel.IsAlowedToBet,
                };

                createPlayerMappingModel.BookiePlayerId = partnerUsername;

                var playerMapping = new CasinoPlayerMapping()
                {
                    PlayerId = createPlayerMappingModel.PlayerId,
                    BookiePlayerId = createPlayerMappingModel.BookiePlayerId,
                    NickName =  createPlayerMappingModel.NickName,
                    IsAccountEnable = createPlayerMappingModel.IsAccountEnable,
                    IsAlowedToBet = createPlayerMappingModel.IsAlowedToBet,
                    CreatedAt = DateTime.Now,
                };

                await _casinoPlayerMappingRepository.AddAsync(playerMapping);
                await LotteryUow.SaveChangesAsync();
            }

            return;
        }

        public override async Task GenerateUrl(object data)
        {
            if (data is null) return;

            var cAPartnerLoginModel = data as CasinoAllBetPlayerLoginModel;
            var _casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var _casinoPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var _casinoAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();

            //Check player is synced
            var cAPlayerMapping = await _casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == cAPartnerLoginModel.PlayerId).FirstOrDefaultAsync();

            if (cAPlayerMapping != null)
            {
                cAPartnerLoginModel.Player = cAPlayerMapping.BookiePlayerId;
                var resultLogin = await _cAService.SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, cAPartnerLoginModel.ToBodyJson());
                await UpdateCacheClientUrl(cAPartnerLoginModel.PlayerId, await resultLogin.Content.ReadAsStringAsync());
                return;
            }
            else
            {
                var cACreatePartnerPlayerModel = new CasinoAllBetPlayerModel()
                {
                    PlayerId = cAPartnerLoginModel.PlayerId,
                    IsAccountEnabled = true,
                    IsAlowedToBet = true,
                };
                await CreatePlayer(cACreatePartnerPlayerModel);

                cAPlayerMapping = await _casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == cAPartnerLoginModel.PlayerId).FirstOrDefaultAsync();

                if (cAPlayerMapping != null)
                {
                    cAPartnerLoginModel.Player = cAPlayerMapping.BookiePlayerId;
                    var resultLogin = await _cAService.SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, cAPartnerLoginModel.ToBodyJson());
                    await UpdateCacheClientUrl(cAPartnerLoginModel.PlayerId, await resultLogin.Content.ReadAsStringAsync());
                    return;
                }
            }
            return;
        }

        public override async Task UpdateBetSetting(object data)
        {
            if (data is null) return;

            var playerBetSetting = data as CasinoAllBetPlayerBetSettingModel;
            var _casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            //Check player is synced
            var cAPlayerMapping = await _casinoPlayerMappingRepository.FindQueryBy(c=>c.PlayerId == playerBetSetting.PlayerId).FirstOrDefaultAsync();

            if (cAPlayerMapping != null)
            {
                var cAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
                playerBetSetting.GeneralHandicaps = await cAgentHandicapRepository.FindQueryBy(c => playerBetSetting.GeneralHandicapIds.Contains(c.Id)).Select(c => c.Name).ToArrayAsync();
                playerBetSetting.VipHandicap = await cAgentHandicapRepository.FindQueryBy(c => playerBetSetting.VipHandicapId == c.Id).Select(c => c.Name).FirstOrDefaultAsync();
                playerBetSetting.Player = cAPlayerMapping.BookiePlayerId;

                var resultModifyPlayerSetting = await _cAService.SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson());
            }
            return;
        }

        private async Task UpdateCacheClientUrl(long playerId, string clientUrl)
        {
            var clientUrlKey = playerId.GetCACientUrlByPlayerId();
            await _redisCacheService.HashSetFieldsAsync(clientUrlKey.MainKey, new Dictionary<string, string>
            {
                { clientUrlKey.SubKey, clientUrl }
            }, clientUrlKey.TimeSpan == TimeSpan.Zero ? null : clientUrlKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        public string[] GetStringGeneralHandicaps(IEnumerable<CasinoAgentHandicap> item)
        {
            if (item == null || !item.Any()) return null;
            return item.Select(c => c.Name).ToArray();
        }
    }
}
