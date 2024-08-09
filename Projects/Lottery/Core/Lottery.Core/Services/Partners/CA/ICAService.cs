using Lottery.Core.Helpers;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
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
    public interface ICAService
    {
        Task<string> SendRequestAsync(HttpMethod httpMethod, string path, string requestBody);
        string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId);
        string Base64edMd5(string data);
        byte[] MD5Hash(byte[] data);
        Task<CABookieSettingValue> GetCABookieSettingValueAsync();
        Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path);
    }
}
