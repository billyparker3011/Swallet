using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Lottery.Core.Services.Partners.CA
{
    public class CAService : LotteryBaseService<CAService>, ICAService
    {
        private readonly HttpClient _client;
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;

        public CAService(
           ILogger<CAService> logger,
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

        public async Task<string> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody)
        {
            try
            {
                var cABookieSetting = await GetCABookieSettingValueAsync();

                CultureInfo ci = new CultureInfo("en-US");
                string requestTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss z", ci);

                string contentMD5 = Base64edMd5(requestBody);
                var authorization = GeneralAuthorizationHeader(httpMethod.Method, path, contentMD5, cABookieSetting.ContentType, requestTime, cABookieSetting.AllbetApiKey, cABookieSetting.OperatorId);

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = httpMethod;
                httpRequestMessage.Content = httpMethod == HttpMethod.Post ? new StringContent(requestBody) : null;

                httpRequestMessage.RequestUri = new Uri(cABookieSetting.ApiURL + path);
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", authorization);
                httpRequestMessage.Headers.TryAddWithoutValidation("Date", requestTime);
                httpRequestMessage.Content.Headers.TryAddWithoutValidation("Content-MD5", contentMD5);
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = _client.SendAsync(httpRequestMessage).Result;
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException e)
            {
                Logger.LogError($"SendRequest failed.");
                return null;
            }
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

        public string Base64edMd5(string data)
        {
            return Convert.ToBase64String(MD5Hash(Encoding.UTF8.GetBytes(data)));
        }

        public byte[] MD5Hash(byte[] data)
        {
            MD5 md5Crp = MD5.Create();
            return md5Crp.ComputeHash(data);
        }

        public async Task<AllbetBookieSettingValue> GetCABookieSettingValueAsync()
        {
            var cABookieSetting = await _redisCacheService.GetDataAsync<AllbetBookieSettingValue>(PartnerHelper.CAPartnerKey.CABookieSettingKey);

            if (cABookieSetting != null)
            {
                return cABookieSetting;
            }
            else
            {
                var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
                cABookieSetting = new AllbetBookieSettingValue();//await bookiesSettingRepos.FindBookieSettingByType(PartnerType.Allbet) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
                await _redisCacheService.SetAddAsync<AllbetBookieSettingValue>(PartnerHelper.CAPartnerKey.CABookieSettingKey, cABookieSetting);
            }

            return cABookieSetting;

        }

        public async Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader)) return false;
            if (string.IsNullOrWhiteSpace(dateHeader)) return false;
            if (string.IsNullOrWhiteSpace(player)) return false;

            var cABookieSettingValue = await GetCABookieSettingValueAsync();

            var signature = authorizationHeader.Substring(authorizationHeader.IndexOf(cABookieSettingValue.OperatorId) + 1);

            if (string.IsNullOrWhiteSpace(signature)) return false;

            var header = GeneralAuthorizationHeader(HttpMethod.Get.Method, path, null, null, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

            if (header == authorizationHeader) return true;

            return false;
        }
    }
}
