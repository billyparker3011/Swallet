using Azure;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
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
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
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
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IRedisCacheService redisCacheService
            ) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow, inMemoryUnitOfWork)
        {
            _redisCacheService = redisCacheService;
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.Allbet;

        public override async Task CreatePlayer(object data)
        {
            Logger.LogInformation($"Start CreatePlayer");
            if (data is null) return;

            var settingValue = await GetBookieSetting();

            var createPartnerPlayerModel = data as CasinoAllBetPlayerModel;
            Logger.LogInformation($"Player {createPartnerPlayerModel.PlayerId}");
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

            //  Check Or Create in partner
            var response = await SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.CheckOrCreate, createPartnerPlayerModel.ToBodyJson(), settingValue);

            // Update player bet setting in partner
            var playerBetSetting = new CasinoAllBetPlayerBetSettingModel()
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

            // Update bet setting in Partner
            await SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson(), settingValue);

            // Create Player Mapping
            var playerMapping = new CasinoPlayerMapping()
            {
                PlayerId = createPartnerPlayerModel.PlayerId,
                BookiePlayerId = partnerUsername,
                NickName = createPartnerPlayerModel.NickName,
                IsAccountEnable = createPartnerPlayerModel.IsAccountEnabled,
                IsAlowedToBet = createPartnerPlayerModel.IsAlowedToBet,
                CreatedAt = ClockService.GetUtcNow(),
            };

            await casinoPlayerMappingRepository.AddAsync(playerMapping);
            await LotteryUow.SaveChangesAsync();

        }

        public override async Task GenerateUrl(object data)
        {
            Logger.LogInformation($"Start GenerateUrl with data {data}");
            if (data is null) return;

            var settingValue = await GetBookieSetting();
            var casinoPartnerLoginModel = data as CasinoAllBetPlayerLoginModel;
            Logger.LogInformation($"Player {casinoPartnerLoginModel.PlayerId}");
            await RemoveCacheClientUrl(casinoPartnerLoginModel.PlayerId);

            // Check player can play
            var isPlay = await CheckMaxWinLose(casinoPartnerLoginModel.PlayerId);
            if(!isPlay) return;

            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();           

            //  Check player is synced
            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == casinoPartnerLoginModel.PlayerId).FirstOrDefaultAsync();
            if (casinoPlayerMapping != null)
            {
                casinoPartnerLoginModel.Player = casinoPlayerMapping.BookiePlayerId;

                var response = await SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, casinoPartnerLoginModel.ToBodyJson(), settingValue);
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

                    var response = await SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.Login, casinoPartnerLoginModel.ToBodyJson(), settingValue);
                    if (response == null) return;
                    await UpdateCacheClientUrl(casinoPartnerLoginModel.PlayerId, await response.Content.ReadAsStringAsync());
                }
            }
        }

        public override async Task UpdateBetSetting(object data)
        {
            Logger.LogInformation($"Start UpdateBetSetting with data {data}");
            if (data is null) return;

            var settingValue = await GetBookieSetting();

            var playerBetSetting = data as CasinoAllBetPlayerBetSettingModel;
            Logger.LogInformation($"Player {playerBetSetting.PlayerId}");
            var casinoPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            //  Check player is synced
            var casinoPlayerMapping = await casinoPlayerMappingRepository.FindQueryBy(c => c.PlayerId == playerBetSetting.PlayerId).FirstOrDefaultAsync();
            if (casinoPlayerMapping == null) return;

            var casinoAgentHandicapRepository = LotteryUow.GetRepository<ICasinoAgentHandicapRepository>();
            playerBetSetting.GeneralHandicaps = await casinoAgentHandicapRepository.FindQueryBy(c => playerBetSetting.GeneralHandicapIds.Contains(c.Id)).Select(c => c.Name).ToArrayAsync();
            playerBetSetting.VipHandicap = await casinoAgentHandicapRepository.FindQueryBy(c => playerBetSetting.VipHandicapId == c.Id).Select(c => c.Name).FirstOrDefaultAsync();
            playerBetSetting.Player = casinoPlayerMapping.BookiePlayerId;

            await SendRequestAsync(HttpMethod.Post, PartnerHelper.CasinoPathPost.ModifyPlayerSetting, playerBetSetting.ToBodyJson(), settingValue);
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

        private async Task RemoveCacheClientUrl(long playerId)
        {
            var clientUrlKey = playerId.GetCasinoClientUrlByPlayerId();
            await _redisCacheService.HashDeleteFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
        }

        private string[] GetStringGeneralHandicaps(IEnumerable<CasinoAgentHandicap> item)
        {
            if (item == null || !item.Any()) return null;
            return item.Select(c => c.Name).ToArray();
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

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody, AllbetBookieSettingValue settingValue)
        {
            try
            {
                HttpClient client = new HttpClient();
                CultureInfo ci = new CultureInfo("en-US");
                string requestTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss z", ci);

                string contentMD5 = Base64edMd5(requestBody);
                var authorization = GeneralAuthorizationHeader(httpMethod.Method, path, contentMD5, settingValue.ContentType, requestTime, settingValue.AllbetApiKey, settingValue.OperatorId);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = httpMethod;
                httpRequestMessage.Content = httpMethod == HttpMethod.Post ? new StringContent(requestBody) : null;

                httpRequestMessage.RequestUri = new Uri(settingValue.ApiURL + path);
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", authorization);
                httpRequestMessage.Headers.TryAddWithoutValidation("Date", requestTime);
                httpRequestMessage.Content.Headers.TryAddWithoutValidation("Content-MD5", contentMD5);
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Logger.LogInformation($"SendRequest to {httpRequestMessage.RequestUri} with body: {requestBody}, header: {httpRequestMessage.Headers}.");
                HttpResponseMessage response = client.SendAsync(httpRequestMessage).Result;
                Logger.LogInformation($"Response: {await response.Content.ReadAsStringAsync()}");
                return response;

            }
            catch (HttpRequestException e)
            {
                Logger.LogError($"SendRequest failed with errors {e.Message}.");
                return null;
            }
        }

        private async Task<bool> CheckMaxWinLose(long playerId)
        {
            if(playerId < 0) return false;


            var casinoPlayerBetSettingRepository = LotteryUow.GetRepository<ICasinoPlayerBetSettingRepository>();
            var casinoAgentBetSettingRepository = LotteryUow.GetRepository<ICasinoAgentBetSettingRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var dateTime = DateTime.UtcNow.AddHours(8);

            var playerBetSetting = await casinoPlayerBetSettingRepository.FindQueryBy(c => c.PlayerId == playerId).FirstOrDefaultAsync();

            if (playerBetSetting != null && (playerBetSetting.MaxWin.HasValue || playerBetSetting.MaxLose.HasValue))
            {                
                var winlossAmount = casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == playerId && !c.IsCancel && c.CreatedAt >= new DateTime(dateTime.Day, dateTime.Month, dateTime.Year) && c.CreatedAt < new DateTime(dateTime.Day, dateTime.Month, dateTime.Year).AddDays(1)).Select(c => c.WinOrLossAmount ?? 0m).Sum();

                if (winlossAmount >= playerBetSetting.MaxWin.Value || winlossAmount <= -1 * playerBetSetting.MaxLose.Value)
                {
                    await UpdateCacheClientUrl(playerId, "Tài khoản bị khống tiền");
                    return false;
                }
            }
            else
            {
                var player = await playerRepository.FindQueryBy(c => c.PlayerId == playerId).FirstOrDefaultAsync();
                if (player != null)
                {
                    var casinoAgentBetSetting = await casinoAgentBetSettingRepository.FindQueryBy(c => c.AgentId == player.AgentId).FirstOrDefaultAsync();
                    if (casinoAgentBetSetting != null && (casinoAgentBetSetting.MaxWin.HasValue || casinoAgentBetSetting.MaxLose.HasValue))
                    {
                        var winlossAmount = casinoTicketBetDetailRepository.FindQueryBy(c => c.CasinoTicket.PlayerId == playerId && !c.IsCancel && c.CreatedAt >= new DateTime(dateTime.Day, dateTime.Month, dateTime.Year) && c.CreatedAt < new DateTime(dateTime.Day, dateTime.Month, dateTime.Year).AddDays(1)).Select(c => c.WinOrLossAmount ?? 0m).Sum();

                        if (winlossAmount >= casinoAgentBetSetting.MaxWin.Value || winlossAmount <= -1 * casinoAgentBetSetting.MaxLose.Value)
                        {
                            await UpdateCacheClientUrl(playerId, "Tài khoản bị khống tiền");
                            return false;
                        }
                    }
                }

            }
            return true;
        }

        public override async Task<Dictionary<string, object>> ScanTickets(Dictionary<string, object> data)
        {
            return new Dictionary<string, object>();
        }
    }
}
