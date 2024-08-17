using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Lottery.Core.Partners.Casino.Allbet
{
    public class CasinoAllbetService : BasePartnerType<CasinoAllbetService>, ICasinoAllbetService
    {
        private readonly IRedisCacheService _redisCacheService;

        public CasinoAllbetService(ILogger<CasinoAllbetService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            IHttpClientFactory httpClientFactory,
            ILotteryUow lotteryUow,
            IRedisCacheService redisCacheService
            ) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow)
        {
            _redisCacheService = redisCacheService;
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.Allbet;

        public override async Task CreatePlayer(object data)
        {
            if (data is null) return;

            var settingValue = await GetBookieSetting();
            var createPartnerPlayerModel = data as CasinoAllBetPlayerModel;

            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var casinoPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var casinoAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();

            //  Check player is synced
            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == createPartnerPlayerModel.PlayerId).FirstOrDefaultAsync();
            if (casinoPlayerMapping != null) return;

            //  Create new player
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(createPartnerPlayerModel.PlayerId) ?? throw new NotFoundException();
            var partnerUsername = GeneralUsername(player.PlayerId.ToString(), settingValue.Suffix).ToLowerInvariant();

            //  Set username and agent
            createPartnerPlayerModel.Player = partnerUsername;
            createPartnerPlayerModel.Agent = settingValue.Agent;

            //  Check or Create in partner
            var httpClient = CreateClient(HttpMethod.Post, PartnerHelper.CasinoPathPost.CheckOrCreate, createPartnerPlayerModel.ToBodyJson(), settingValue);
            var url = $"{settingValue.ApiURL}{PartnerHelper.CasinoPathPost.CheckOrCreate}";
            var content = new StringContent(createPartnerPlayerModel.ToBodyJson(), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);

            //  Update player bet setting in partner
            var playerBetSetting = new CasinoAllBetPlayerBetSettingModel
            {
                Player = partnerUsername,
                Nickname = createPartnerPlayerModel.NickName,
                IsAccountEnabled = createPartnerPlayerModel.IsAccountEnabled,
                IsAllowedToBet = createPartnerPlayerModel.IsAlowedToBet,
            };

            //  Get bet setting
            var casinoPlayerBetSettings = await casinoPlayerBetSettingRepository.FindQueryBy(c => c.PlayerId == createPartnerPlayerModel.PlayerId).Include(c => c.CasinoPlayerBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.CasinoAgentHandicap).Include(c => c.Player).Include(c => c.CasinoBetKind).ToListAsync();
            if (casinoPlayerBetSettings != null && casinoPlayerBetSettings.Any())
            {
                var casinoPlayerBetSetting = casinoPlayerBetSettings.FirstOrDefault();
                playerBetSetting.GeneralHandicaps = GetStringGeneralHandicaps(casinoPlayerBetSetting.CasinoPlayerBetSettingAgentHandicaps.Select(c => c.CasinoAgentHandicap));
                playerBetSetting.VipHandicap = casinoPlayerBetSetting.CasinoAgentHandicap.Name;
            }
            else
            {
                var casinoAgentBetSettings = await casinoAgentBetSettingRepository.FindQueryBy(c => c.AgentId == player.AgentId).Include(c => c.CasinoAgentBetSettingAgentHandicaps).ThenInclude(c => c.CasinoAgentHandicap).Include(c => c.DefaultVipHandicap).Include(c => c.Agent).Include(c => c.BetKind).ToListAsync();
                if (casinoAgentBetSettings != null && casinoAgentBetSettings.Any())
                {
                    var casinoAgentBetSetting = casinoAgentBetSettings.FirstOrDefault();
                    playerBetSetting.GeneralHandicaps = GetStringGeneralHandicaps(casinoAgentBetSetting.CasinoAgentBetSettingAgentHandicaps.Select(c => c.CasinoAgentHandicap));
                    playerBetSetting.VipHandicap = casinoAgentBetSetting.DefaultVipHandicap.Name;
                }
            }

            httpClient = CreateClient(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson(), settingValue);
            url = $"{settingValue.ApiURL}{PartnerHelper.CasinoPathPost.ModifyPlayerSetting}";
            content = new StringContent(playerBetSetting.ToBodyJson(), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);

            //  Create Player Mapping
            var createPlayerMappingModel = new CreateCasinoPlayerMappingModel
            {
                PlayerId = createPartnerPlayerModel.PlayerId,
                NickName = createPartnerPlayerModel.NickName,
                IsAccountEnable = createPartnerPlayerModel.IsAccountEnabled,
                IsAlowedToBet = createPartnerPlayerModel.IsAlowedToBet,
                BookiePlayerId = partnerUsername
            };

            var playerMapping = new CasinoPlayerMapping
            {
                PlayerId = createPlayerMappingModel.PlayerId,
                BookiePlayerId = createPlayerMappingModel.BookiePlayerId,
                NickName = createPlayerMappingModel.NickName,
                IsAccountEnable = createPlayerMappingModel.IsAccountEnable,
                IsAlowedToBet = createPlayerMappingModel.IsAlowedToBet,
                CreatedAt = ClockService.GetUtcNow()
            };

            await casinoPlayerMappingRepository.AddAsync(playerMapping);
            await LotteryUow.SaveChangesAsync();
        }

        public override async Task GenerateUrl(object data)
        {
            if (data is null) return;

            var settingValue = await GetBookieSetting();
            var casinoPartnerLoginModel = data as CasinoAllBetPlayerLoginModel;

            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();
            var casinoPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var casinoAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();

            //  Check player is synced
            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == casinoPartnerLoginModel.PlayerId).FirstOrDefaultAsync();
            if (casinoPlayerMapping != null)
            {
                casinoPartnerLoginModel.Player = casinoPlayerMapping.BookiePlayerId;

                var httpClient = CreateClient(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, casinoPartnerLoginModel.ToBodyJson(), settingValue);
                var url = $"{settingValue.ApiURL}{PartnerHelper.CasinoPathPost.Login}";
                var content = new StringContent(casinoPartnerLoginModel.ToBodyJson(), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                if (response == null) return;
                await UpdateCacheClientUrl(casinoPartnerLoginModel.PlayerId, await response.Content.ReadAsStringAsync());
            }
            else
            {
                var casinoCreatePartnerPlayerModel = new CasinoAllBetPlayerModel
                {
                    PlayerId = casinoPartnerLoginModel.PlayerId,
                    IsAccountEnabled = true,
                    IsAlowedToBet = true,
                };
                await CreatePlayer(casinoCreatePartnerPlayerModel);

                casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == casinoPartnerLoginModel.PlayerId).FirstOrDefaultAsync();
                if (casinoPlayerMapping != null)
                {
                    casinoPartnerLoginModel.Player = casinoPlayerMapping.BookiePlayerId;

                    var httpClient = CreateClient(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, casinoPartnerLoginModel.ToBodyJson(), settingValue);
                    var url = $"{settingValue.ApiURL}{PartnerHelper.CasinoPathPost.Login}";
                    var content = new StringContent(casinoPartnerLoginModel.ToBodyJson(), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(url, content);
                    if (response == null) return;
                    await UpdateCacheClientUrl(casinoPartnerLoginModel.PlayerId, await response.Content.ReadAsStringAsync());
                }
            }
        }

        public override async Task UpdateBetSetting(object data)
        {
            if (data is null) return;

            var settingValue = await GetBookieSetting();
            var playerBetSetting = data as CasinoAllBetPlayerBetSettingModel;
            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            //  Check player is synced
            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == playerBetSetting.PlayerId).FirstOrDefaultAsync();
            if (casinoPlayerMapping == null) return;

            var casinoAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            playerBetSetting.GeneralHandicaps = await casinoAgentHandicapRepository.FindQueryBy(c => playerBetSetting.GeneralHandicapIds.Contains(c.Id)).Select(c => c.Name).ToArrayAsync();
            playerBetSetting.VipHandicap = await casinoAgentHandicapRepository.FindQueryBy(c => playerBetSetting.VipHandicapId == c.Id).Select(c => c.Name).FirstOrDefaultAsync();
            playerBetSetting.Player = casinoPlayerMapping.BookiePlayerId;

            var httpClient = CreateClient(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson(), settingValue);
            var url = $"{settingValue.ApiURL}{PartnerHelper.CasinoPathPost.ModifyPlayerSetting}";
            var content = new StringContent(playerBetSetting.ToBodyJson(), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);
        }

        private async Task<AllbetBookieSettingValue> GetBookieSetting()
        {
            var bookiesSettingRepository = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var casinoSetting = await bookiesSettingRepository.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (string.IsNullOrEmpty(casinoSetting.SettingValue)) throw new NotFoundException();
            return JsonConvert.DeserializeObject<AllbetBookieSettingValue>(casinoSetting.SettingValue);
        }

        private async Task UpdateCacheClientUrl(long playerId, string clientUrl)
        {
            var clientUrlKey = playerId.GetCasinoClientUrlByPlayerId();
            await _redisCacheService.HashSetFieldsAsync(clientUrlKey.MainKey, new Dictionary<string, string>
            {
                { clientUrlKey.SubKey, clientUrl }
            }, clientUrlKey.TimeSpan == TimeSpan.Zero ? null : clientUrlKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        private string[] GetStringGeneralHandicaps(IEnumerable<CasinoAgentHandicap> item)
        {
            if (item == null || !item.Any()) return null;
            return item.Select(c => c.Name).ToArray();
        }

        private HttpClient CreateClient(HttpMethod httpMethod, string path, string requestBody, AllbetBookieSettingValue setting)
        {
            var httpClient = HttpClientFactory.CreateClient();

            var ci = new CultureInfo("en-US");
            var requestTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss z", ci);

            var contentMD5 = Base64edMd5(requestBody);
            var authorization = GeneralAuthorizationHeader(httpMethod.Method, path, contentMD5, setting.ContentType, requestTime, setting.AllbetApiKey, setting.OperatorId);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Date", requestTime);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-MD5", contentMD5);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(HttpRequestHeader.ContentType.ToString(), "application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private string Base64edMd5(string data)
        {
            return Convert.ToBase64String(MD5Hash(Encoding.UTF8.GetBytes(data)));
        }

        private byte[] MD5Hash(byte[] data)
        {
            MD5 md5Crp = MD5.Create();
            return md5Crp.ComputeHash(data);
        }

        private string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId)
        {
            var stringToSign
             = httpMethod + "\n"
             + contentMD5 + "\n"
             + contentType + "\n"
             + requestTime + "\n"
             + path;

            var encrypted = HashSignature(allbetApiKey, stringToSign);
            return "AB" + " " + operatorId + ":" + encrypted;
        }

        private string HashSignature(string key, string value)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Convert.FromBase64String(key);
            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashBytes);
        }

        private string GeneralUsername(string playerId, string suffix)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(playerId);
            return Convert.ToBase64String(plainTextBytes).Substring(0, playerId.Length < 12 ? playerId.Length : 12) + GenerateRandomString() + suffix;
        }

        private string GenerateRandomString(int length = 4)
        {
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = GenerateRandomChar();
            }
            return new string(result);
        }

        private char GenerateRandomChar()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int index = new Random().Next(chars.Length - 1);
            return chars[index];
        }
    }
}
