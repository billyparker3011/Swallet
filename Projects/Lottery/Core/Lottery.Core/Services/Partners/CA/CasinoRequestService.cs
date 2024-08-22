using Azure.Core;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Lottery.Core.Helpers.PartnerHelper;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoRequestService : ICasinoRequestService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CasinoRequestService> Logger;
        private readonly ICasinoBookieSettingService _casinoBookieSettingService;
        private readonly ILotteryUow _lotteryUow;
        public CasinoRequestService(HttpClient client,
            ILogger<CasinoRequestService> logger,
            ICasinoBookieSettingService casinoBookieSettingService,
            ILotteryUow lotteryUow
            )
        {
            _client = client;
            Logger = logger;
            _casinoBookieSettingService = casinoBookieSettingService;
            _lotteryUow = lotteryUow;
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

        public async Task<int> ValidateHeader(HttpRequest request, string requestBody)
        {
            var path = request.RouteValues["action"]?.ToString();
            request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            request.Headers.TryGetValue("Date", out var dateHeader);
            request.Headers.TryGetValue("Content-MD5", out var contentMD5);
            request.Headers.TryGetValue("Content-Type", out var contentType);
            request.RouteValues.TryGetValue("player", out var player);

            if (string.IsNullOrWhiteSpace(authorizationHeader)) return CasinoReponseCode.Invalid_Signature;
            if (string.IsNullOrWhiteSpace(dateHeader)) return CasinoReponseCode.Invalid_request_parameter;

            var cABookieSettingValue = await _casinoBookieSettingService.GetCasinoBookieSettingValueAsync();

            if (!authorizationHeader.ToString().Contains($"AB {cABookieSettingValue.OperatorId}:")) return CasinoReponseCode.Invalid_Operator_ID;

            if (path.ToLowerInvariant() == CasinoPartnerPath.GetBalance)
            {

                var header = GeneralAuthorizationHeader(HttpMethod.Get.Method, $"/{path}/{player}", null, contentType, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

                if (header != authorizationHeader) return CasinoReponseCode.Invalid_Signature;
            }

            if (path.ToLowerInvariant() == CasinoPartnerPath.Transfer || path.ToLowerInvariant() == CasinoPartnerPath.CancelTransfer)
            {
                var cMD5 = Base64edMd5(requestBody);

                if (cMD5 != contentMD5) return CasinoReponseCode.Invalid_Signature;

                var header = GeneralAuthorizationHeader(HttpMethod.Post.Method, "/" + path, cMD5, contentType, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

                if (header != authorizationHeader) return CasinoReponseCode.Invalid_Signature;

            }

            return CasinoReponseCode.Success;
        }

        public async Task<int> ValidateHeader(HttpRequest request)
        {
            var path = request.RouteValues["action"]?.ToString();
            request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            request.Headers.TryGetValue("Date", out var dateHeader);
            request.Headers.TryGetValue("Content-MD5", out var contentMD5);
            request.Headers.TryGetValue("Content-Type", out var contentType);

            if (string.IsNullOrWhiteSpace(authorizationHeader)) return CasinoReponseCode.Invalid_Signature;
            if (string.IsNullOrWhiteSpace(dateHeader)) return CasinoReponseCode.Invalid_request_parameter;
          
            var cABookieSettingValue = await _casinoBookieSettingService.GetCasinoBookieSettingValueAsync();

            if (!authorizationHeader.ToString().Contains($"AB {cABookieSettingValue.OperatorId}:")) return CasinoReponseCode.Invalid_Operator_ID;  

            if (path.ToLowerInvariant() == CasinoPartnerPath.GetBalance)
            {
                var header = GeneralAuthorizationHeader(HttpMethod.Get.Method, "/" + path, null, null, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

                if (header != authorizationHeader) return CasinoReponseCode.Invalid_Signature;
            }

            if (path.ToLowerInvariant() == CasinoPartnerPath.Transfer || path.ToLowerInvariant() == CasinoPartnerPath.CancelTransfer)
            {
                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                var requestBody = Encoding.UTF8.GetString(buffer);
                request.Body.Position = 0; 

                var cMD5 = Base64edMd5(requestBody);

                if (cMD5 != contentMD5) return CasinoReponseCode.Invalid_Signature;

                var header = GeneralAuthorizationHeader(HttpMethod.Post.Method, "/" + path, cMD5, contentType, dateHeader, cABookieSettingValue.PartnerApiKey, cABookieSettingValue.OperatorId);

                if (header != authorizationHeader) return CasinoReponseCode.Invalid_Signature;

            }

            return CasinoReponseCode.Success;
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
