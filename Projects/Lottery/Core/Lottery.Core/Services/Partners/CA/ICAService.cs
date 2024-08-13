using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICAService
    {
        Task<string> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody);
        string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId);
        string Base64edMd5(string data);
        byte[] MD5Hash(byte[] data);
        Task<AllbetBookieSettingValue> GetCABookieSettingValueAsync();
        Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path);
    }
}
