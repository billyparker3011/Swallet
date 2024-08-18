using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoRequestService : ICasinoRequestService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CasinoRequestService> Logger;
        private readonly ICasinoBookieSettingService _casinoBookieSettingService;
        public CasinoRequestService(HttpClient client,
            ILogger<CasinoRequestService> logger,
            ICasinoBookieSettingService casinoBookieSettingService
            )
        {
            _client = client;
            Logger = logger;
            _casinoBookieSettingService = casinoBookieSettingService;
        }
        public async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody)
        {
            try
            {
                var cABookieSetting = await _casinoBookieSettingService.GetCasinoBookieSettingValueAsync();

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

                Logger.LogInformation($"SendRequest to {httpRequestMessage.RequestUri} with body: {requestBody}, header: {httpRequestMessage.Headers}.");
                HttpResponseMessage response = _client.SendAsync(httpRequestMessage).Result;
                Logger.LogInformation($"Response: {response.Content}");
                return response;

            }
            catch (HttpRequestException e)
            {
                Logger.LogError($"SendRequest failed with errors {e.Message}.");
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


        private string HashSignature(string key, string value)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Convert.FromBase64String(key);
            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashBytes);
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

        private async Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader)) return false;
            if (string.IsNullOrWhiteSpace(dateHeader)) return false;
            if (string.IsNullOrWhiteSpace(player)) return false;

            var cABookieSettingValue = await _casinoBookieSettingService.GetCasinoBookieSettingValueAsync();

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
