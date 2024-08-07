using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace Lottery.Core.Partners.CockFight.GA28
{
    public class CockFightGa28Service : BasePartnerType
    {
        public override PartnerType PartnerType => PartnerType.GA28;
        private readonly HttpClient _httpClient;
        private readonly IRedisCacheService _redisCacheService;

        public CockFightGa28Service(ILogger<CockFightGa28Service> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryUow lotteryUow, HttpClient httpClient, IRedisCacheService redisCacheService) : base(logger, serviceProvider, configuration, clockService, lotteryUow)
        {
            _httpClient = httpClient;
            _redisCacheService = redisCacheService;
        }

        public override async Task<HttpResponseMessage> CreatePlayer(object data)
        {
            if (data is null) return null;
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();
            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", cockFightRequestSetting.SettingValue.AuthValue);

            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/";
            var response = await _httpClient.PostAsync(url, content);
            return response;

        }

        public override async Task UpdateBetSetting(object data)
        {
            if (data is null) return;

            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();

            var ga28BetSettingData = data as Ga28SyncUpBetSettingModel;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", cockFightRequestSetting.SettingValue.AuthValue);
            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/{ga28BetSettingData.MemberRefId.ToLower()}";
            // Update limit amount per fight setting
            var limitAmountPerFightSetting = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                main_limit_amount_per_fight = ga28BetSettingData.MainLimitAmountPerFight,
                draw_limit_amount_per_fight = ga28BetSettingData.DrawLimitAmountPerFight
            });
            var limitAmountContent = new StringContent(limitAmountPerFightSetting, Encoding.UTF8, "application/json");

            await _httpClient.PatchAsync(url, limitAmountContent);

            // Update limit number ticket per fight setting
            var limitNumbTicketPerFightSetting = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                limit_num_ticket_per_fight = ga28BetSettingData.LimitNumTicketPerFight
            });
            var limitNumbTicketContent = new StringContent(limitNumbTicketPerFightSetting, Encoding.UTF8, "application/json");

            await _httpClient.PatchAsync(url, limitNumbTicketContent);
        }

        public override async Task GenerateUrl(object data)
        {
            if (data is null) return;

            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", cockFightRequestSetting.SettingValue.AuthValue);

            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/login";
            var response = await _httpClient.PostAsync(url, content);

            if (response is null) return;

            var stringData = await response.Content.ReadAsStringAsync();
            var objectData = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28LoginPlayerDataReturnModel>(stringData);
            if (objectData is null || objectData.Member is null) return;

            var playerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.MemberRefId == objectData.Member.MemberRefId && x.IsInitial).FirstOrDefaultAsync();
            if (playerMapping is null) return;

            // Update cache
            await UpdateCacheClientUrl(playerMapping.PlayerId, objectData.GameClientUrl);
            await UpdateCacheToken(playerMapping.PlayerId, objectData.Token);
        }

        private async Task UpdateCacheClientUrl(long playerId, string clientUrl)
        {
            var clientUrlKey = playerId.GetGa28ClientUrlByPlayerId();
            await _redisCacheService.HashSetFieldsAsync(clientUrlKey.MainKey, new Dictionary<string, string>
            {
                { clientUrlKey.SubKey, clientUrl }
            }, clientUrlKey.TimeSpan == TimeSpan.Zero ? null : clientUrlKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        private async Task UpdateCacheToken(long playerId, string token)
        {
            var tokenKey = playerId.GetGa28TokenByPlayerId();
            await _redisCacheService.HashSetFieldsAsync(tokenKey.MainKey, new Dictionary<string, string>
            {
                { tokenKey.SubKey, token }
            }, tokenKey.TimeSpan == TimeSpan.Zero ? null : tokenKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }
    }
}
