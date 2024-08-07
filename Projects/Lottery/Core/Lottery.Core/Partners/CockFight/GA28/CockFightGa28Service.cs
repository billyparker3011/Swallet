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
        private readonly IRedisCacheService _redisCacheService;

        public CockFightGa28Service(ILogger<BasePartnerType> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow)
        {
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.GA28;

        public override async Task CreatePlayer(object data)
        {
            if (data is null) return;
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();

            var ga28CreatePlayerModel = data as Ga28CreateMemberModel;

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                member_ref_id = ga28CreatePlayerModel.MemberRefId,
                account_id = ga28CreatePlayerModel.AccountId
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpClient = CreateClient(cockFightRequestSetting.SettingValue.AuthValue);

            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/";
            await httpClient.PostAsync(url, content);
        }

        private HttpClient CreateClient(string token)
        {
            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Token {token}");
            return httpClient;
        }

        public override async Task UpdateBetSetting(object data)
        {
            if (data is null) return;

            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();

            var httpClient = CreateClient(cockFightRequestSetting.SettingValue.AuthValue);

            var ga28BetSettingData = data as Ga28SyncUpBetSettingModel;

            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/{ga28BetSettingData.MemberRefId.ToLower()}";

            // Update limit amount per fight setting
            var limitAmountPerFightSetting = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                main_limit_amount_per_fight = ga28BetSettingData.MainLimitAmountPerFight,
                draw_limit_amount_per_fight = ga28BetSettingData.DrawLimitAmountPerFight
            });
            var limitAmountContent = new StringContent(limitAmountPerFightSetting, Encoding.UTF8, "application/json");

            await httpClient.PatchAsync(url, limitAmountContent);

            // Update limit number ticket per fight setting
            var limitNumbTicketPerFightSetting = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                limit_num_ticket_per_fight = ga28BetSettingData.LimitNumTicketPerFight
            });
            var limitNumbTicketContent = new StringContent(limitNumbTicketPerFightSetting, Encoding.UTF8, "application/json");

            await httpClient.PatchAsync(url, limitNumbTicketContent);
        }

        public override async Task GenerateUrl(object data)
        {
            if (data is null) return;

            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (cockFightRequestSetting.SettingValue == null) throw new NotFoundException();

            var ga28GenerateUrlModel = data as Ga28LoginPlayerModel;

            var httpClient = CreateClient(cockFightRequestSetting.SettingValue.AuthValue);

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                member_ref_id = ga28GenerateUrlModel.MemberRefId,
                account_id = ga28GenerateUrlModel.AccountId
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var url = $"{cockFightRequestSetting.SettingValue.ApiAddress}/api/v1/members/login";
            var response = await httpClient.PostAsync(url, content);
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
