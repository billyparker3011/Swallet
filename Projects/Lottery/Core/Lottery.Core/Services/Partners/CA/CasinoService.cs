using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;


namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoService : LotteryBaseService<CasinoService>, ICasinoService
    {
        private readonly HttpClient _client;
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CasinoService(
           ILogger<CasinoService> logger,
           IServiceProvider serviceProvider,
           IConfiguration configuration,
           IClockService clockService,
           ILotteryClientContext clientContext,
           ILotteryUow lotteryUow,
           IPartnerPublishService partnerPublishService,
           IRedisCacheService redisCacheService,
           HttpClient client)
           : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _client = client;
        }

        public async Task AllBetPlayerLoginAsync(CasinoAllBetPlayerLoginModel model)
        {
            model.PlayerId = ClientContext.Player.PlayerId;
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task CreateAllBetPlayerAsync(CasinoAllBetPlayerModel model)
        {
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task UpdateAllBetPlayerBetSettingAsync(CasinoAllBetPlayerBetSettingModel model)
        {
            await _partnerPublishService.Publish(model);
            return;
        }

        public async Task<AllbetBookieSettingValue> GetCasinoBookieSettingValueAsync()
        {
            var data = await _redisCacheService.HashGetAsync(PartnerHelper.CasinoPartnerKey.CasinoBookieSettingKey, CachingConfigs.RedisConnectionForApp);

            if (data != null && data.TryGetValue(nameof(AllbetBookieSettingValue), out string allbetBookieSettingValue))
            {
                return JsonConvert.DeserializeObject<AllbetBookieSettingValue>(allbetBookieSettingValue);
            }
            else
            {
                var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
                var bookieSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.Allbet) ?? throw new NotFoundException();
                var dict = new Dictionary<string, string>() { { nameof(AllbetBookieSettingValue), bookieSetting.SettingValue } };
                await _redisCacheService.HashSetAsync(PartnerHelper.CasinoPartnerKey.CasinoBookieSettingKey, dict, null, CachingConfigs.RedisConnectionForApp);
                return JsonConvert.DeserializeObject<AllbetBookieSettingValue>(bookieSetting.SettingValue);
            }

        }

        public async Task<string> GetGameUrlAsync()
        {
            var caPlayerMappingRepository = LotteryUow.GetRepository<ICasinoPlayerMappingRepository>();

            var cAPlayerMapping = await caPlayerMappingRepository.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            if (cAPlayerMapping == null) return null;

            var clientUrlKey = cAPlayerMapping.PlayerId.GetCasinoClientUrlByPlayerId();
            var clientUrlHash = await _redisCacheService.HashGetFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (!clientUrlHash.TryGetValue(clientUrlKey.SubKey, out string gameUrl)) return null;
            return gameUrl;
        }

        public string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId)
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

        public string HashSignature(string key, string value)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Convert.FromBase64String(key);
            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashBytes);
        }

        public async Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader)) return false;
            if (string.IsNullOrWhiteSpace(dateHeader)) return false;
            if (string.IsNullOrWhiteSpace(player)) return false;

            var cABookieSettingValue = await GetCasinoBookieSettingValueAsync();

            var signature = authorizationHeader.Substring(authorizationHeader.IndexOf(cABookieSettingValue.OperatorId) + 1);

            if (string.IsNullOrWhiteSpace(signature)) return false;

            var header = GeneralAuthorizationHeader(HttpMethod.Get.Method, path, null, null, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

            if (header == authorizationHeader) return true;

            return false;
        }

        public string GeneralUsername(string playerId, string suffix)
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