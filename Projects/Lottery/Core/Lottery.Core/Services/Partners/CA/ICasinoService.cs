﻿using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoService : IScopedDependency
    {
        Task AllBetPlayerLoginAsync(CasinoAllBetPlayerLoginModel model);
        Task CreateAllBetPlayerAsync(CasinoAllBetPlayerModel model);
        Task UpdateAllBetPlayerBetSettingAsync(CasinoAllBetPlayerBetSettingModel model);
        string GeneralAuthorizationHeader(string httpMethod, string path, string contentMD5, string contentType, string requestTime, string allbetApiKey, string operatorId);
        Task<AllbetBookieSettingValue> GetCasinoBookieSettingValueAsync();
        Task<bool> ValidateHeader(string authorizationHeader, string dateHeader, string player, string path);
        string GeneralUsername(string playerId, string suffix);
        Task<string> GetGameUrlAsync();
    }
}