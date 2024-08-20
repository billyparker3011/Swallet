using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.Bookies;
using Lottery.Core.InMemory.Partner;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Lottery.Core.Partners.CockFight.GA28
{
    public class CockFightGa28Service : BasePartnerType<CockFightGa28Service>, ICockFightGa28Service
    {
        private const int _intervalTimeInSeconds = 30;
        private readonly IRedisCacheService _redisCacheService;

        public CockFightGa28Service(ILogger<CockFightGa28Service> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork, IRedisCacheService redisCacheService) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow, inMemoryUnitOfWork)
        {
            _redisCacheService = redisCacheService;
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.GA28;

        private Ga28BookieSettingValue GetBookieSettingFromInMemory()
        {
            var bookieSettingInMemoryRepository = InMemoryUnitOfWork.GetRepository<IBookieSettingInMemoryRepository>();
            var cockFightSetting = bookieSettingInMemoryRepository.FindBy(f => f.BookieTypeId == PartnerType).FirstOrDefault() ?? throw new NotFoundException();
            if (string.IsNullOrEmpty(cockFightSetting.SettingValue)) throw new NotFoundException();
            return JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightSetting.SettingValue);
        }

        private async Task<(Data.Entities.BookieSetting, Ga28BookieSettingValue)> GetBookieSetting()
        {
            var bookiesSettingRepository = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightRequestSetting = await bookiesSettingRepository.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new NotFoundException();
            return (cockFightRequestSetting, JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue));
        }

        public override async Task CreatePlayer(object data)
        {
            if (data is null) return;
            var settingValue = GetBookieSettingFromInMemory();
            var ga28CreatePlayerModel = data as Ga28CreateMemberModel;
            var jsonContent = JsonConvert.SerializeObject(new
            {
                member_ref_id = ga28CreatePlayerModel.MemberRefId,
                account_id = ga28CreatePlayerModel.AccountId
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpClient = CreateClient(settingValue.AuthValue);
            var url = $"{settingValue.ApiAddress}/api/v1/members/";
            await httpClient.PostAsync(url, content);
        }

        private HttpClient CreateClient(string token)
        {
            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
            return httpClient;
        }

        public override async Task UpdateBetSetting(object data)
        {
            if (data is null) return;
            var settingValue = GetBookieSettingFromInMemory();
            var httpClient = CreateClient(settingValue.AuthValue);
            var ga28BetSettingData = data as Ga28SyncUpBetSettingModel;

            var url = $"{settingValue.ApiAddress}/api/v1/members/{ga28BetSettingData.MemberRefId.ToLower()}";
            // Update limit amount per fight setting
            var limitAmountPerFightSetting = JsonConvert.SerializeObject(new
            {
                main_limit_amount_per_fight = ga28BetSettingData.MainLimitAmountPerFight,
                draw_limit_amount_per_fight = ga28BetSettingData.DrawLimitAmountPerFight
            });
            var limitAmountContent = new StringContent(limitAmountPerFightSetting, Encoding.UTF8, "application/json");

            await httpClient.PatchAsync(url, limitAmountContent);

            // Update limit number ticket per fight setting
            var limitNumbTicketPerFightSetting = JsonConvert.SerializeObject(new
            {
                limit_num_ticket_per_fight = ga28BetSettingData.LimitNumTicketPerFight
            });
            var limitNumbTicketContent = new StringContent(limitNumbTicketPerFightSetting, Encoding.UTF8, "application/json");

            await httpClient.PatchAsync(url, limitNumbTicketContent);
        }

        public override async Task GenerateUrl(object data)
        {
            if (data is null) return;
            var settingValue = GetBookieSettingFromInMemory();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var ga28GenerateUrlModel = data as Ga28LoginPlayerModel;

            var httpClient = CreateClient(settingValue.AuthValue);

            var jsonContent = JsonConvert.SerializeObject(new
            {
                member_ref_id = ga28GenerateUrlModel.MemberRefId,
                account_id = ga28GenerateUrlModel.AccountId
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var url = $"{settingValue.ApiAddress}/api/v1/members/login";
            var response = await httpClient.PostAsync(url, content);
            if (response is null) return;

            var stringData = await response.Content.ReadAsStringAsync();
            if (!stringData.IsValidJson()) return;

            Logger.LogInformation($"Login responsed data: {stringData}");

            var objectData = JsonConvert.DeserializeObject<Ga28LoginPlayerDataReturnModel>(stringData);
            if (objectData is null || objectData.Member is null) return;

            Logger.LogInformation($"MemberRefId responsed data: {objectData.Member.MemberRefId}");

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

        public override async Task<Dictionary<string, object>> ScanTickets(Dictionary<string, object> metadata)
        {
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var cockFightTicketRepos = LotteryUow.GetRepository<ICockFightTicketRepository>();

            var cockFightBetKindInMemoryRepository = InMemoryUnitOfWork.GetRepository<ICockFightBetKindInMemoryRepository>();
            var defaultBetKind = cockFightBetKindInMemoryRepository.GetDefaultBetKind() ?? throw new HnMicroException();

            (var cockFightRequestSetting, var settingValue) = await GetBookieSetting();
            var fromModifiedDate = settingValue != null && !string.IsNullOrEmpty(settingValue.ScanTicketTime) ? settingValue.ScanTicketTime : DateTime.UtcNow.ToString();
            var toModifiedDate = DateTime.Parse(fromModifiedDate).AddSeconds(_intervalTimeInSeconds).ToString();

            // Update scan ticket time
            settingValue.ScanTicketTime = toModifiedDate;
            cockFightRequestSetting.SettingValue = JsonConvert.SerializeObject(settingValue);

            // Get Ga28 tickets
            var httpClient = CreateClient(settingValue.AuthValue);
            var ticketParams = new Dictionary<string, string>
            {
                {"from_modified_date_time", fromModifiedDate },
                {"to_modified_date_time", toModifiedDate }
            };

            var baseUrl = $"{settingValue.ApiAddress}/api/v1/tickets/";
            var uri = QueryHelpers.AddQueryString(baseUrl, ticketParams);

            var response = await httpClient.GetAsync(uri);
            if (response is null) return new Dictionary<string, object>();

            var stringData = await response.Content.ReadAsStringAsync();
            if (!stringData.IsValidJson()) return new Dictionary<string, object>();

            Logger.LogInformation($"Login responsed data: {stringData}");

            var listResponseData = JsonConvert.DeserializeObject<List<Ga28RetrieveTicketDataReturnModel>>(stringData);

            // AddNew or Update system tickets
            var listMemberResponseMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => listResponseData.Select(r => r.MemberRefId).Contains(x.MemberRefId)).ToListAsync();
            var players = await playerRepos.FindQueryBy(x => listMemberResponseMapping.Select(p => p.PlayerId).Distinct().Contains(x.PlayerId)).ToListAsync();

            var responseTicketSid = listResponseData.Select(x => x.Sid).ToList();
            var existingTickets = await cockFightTicketRepos.FindQueryBy(x => responseTicketSid.Contains(x.Sid)).ToListAsync();
            var listAddingItem = new List<CockFightTicket>();
            foreach (var itemData in listResponseData)
            {
                var targetPlayerId = listMemberResponseMapping.FirstOrDefault(x => x.MemberRefId == itemData.MemberRefId)?.PlayerId;
                var targetPlayer = players.FirstOrDefault(x => x.PlayerId == targetPlayerId);
                if (targetPlayer == null) continue;
                var updatedTicket = existingTickets.FirstOrDefault(x => x.Sid == itemData.Sid && x.PlayerId == targetPlayer.PlayerId);
                if (updatedTicket != null)
                {
                    // Update existing ticket
                    updatedTicket.AnteAmount = !string.IsNullOrEmpty(itemData.AnteAmount) && decimal.TryParse(itemData.AnteAmount, out var anteAmount) ? anteAmount : null;
                    updatedTicket.ArenaCode = itemData.ArenaCode;
                    updatedTicket.BetAmount = !string.IsNullOrEmpty(itemData.BetAmount) && decimal.TryParse(itemData.BetAmount, out var betAmount) ? betAmount : null;
                    updatedTicket.CurrencyCode = itemData.CurrencyCode;
                    updatedTicket.FightNumber = itemData.FightNumber;
                    updatedTicket.MatchDayCode = itemData.MatchDayCode;
                    updatedTicket.Odds = !string.IsNullOrEmpty(itemData.Odds) && decimal.TryParse(itemData.Odds, out var odds) ? odds : null;
                    updatedTicket.Result = itemData.Result.ToTicketResult();
                    updatedTicket.WinlossAmount = !string.IsNullOrEmpty(itemData.WinLossAmount) && decimal.TryParse(itemData.WinLossAmount, out var winlossAmount) ? winlossAmount : null;
                    updatedTicket.SettledDateTime = itemData.SettledDateTime;
                    updatedTicket.IpAddress = itemData.IpAddress;
                    updatedTicket.UserAgent = itemData.UserAgent;
                    updatedTicket.TicketAmount = !string.IsNullOrEmpty(itemData.TicketAmount) && decimal.TryParse(itemData.TicketAmount, out var ticketAmount) ? ticketAmount : null;
                    updatedTicket.Status = itemData.Status;
                    updatedTicket.TicketModifiedDate = itemData.ModifiedDateTime;
                    updatedTicket.Selection = itemData.Selection.ToCockFightSelection().ToString();
                    updatedTicket.OddType = itemData.OddsType;
                    updatedTicket.ValidStake = !string.IsNullOrEmpty(itemData.ValidStake) && decimal.TryParse(itemData.ValidStake, out var validStake) ? validStake : null;
                }
                else
                {
                    // AddNew ticket
                    listAddingItem.Add(new CockFightTicket
                    {
                        PlayerId = targetPlayer.PlayerId,
                        AgentId = targetPlayer.AgentId,
                        MasterId = targetPlayer.MasterId,
                        SupermasterId = targetPlayer.SupermasterId,
                        Sid = itemData.Sid,
                        AnteAmount = !string.IsNullOrEmpty(itemData.AnteAmount) && decimal.TryParse(itemData.AnteAmount, out var anteAmount) ? anteAmount : null,
                        ArenaCode = itemData.ArenaCode,
                        BetAmount = !string.IsNullOrEmpty(itemData.BetAmount) && decimal.TryParse(itemData.BetAmount, out var betAmount) ? betAmount : null,
                        CurrencyCode = itemData.CurrencyCode,
                        FightNumber = itemData.FightNumber,
                        MatchDayCode = itemData.MatchDayCode,
                        Odds = !string.IsNullOrEmpty(itemData.Odds) && decimal.TryParse(itemData.Odds, out var odds) ? odds : null,
                        Result = itemData.Result.ToTicketResult(),
                        WinlossAmount = !string.IsNullOrEmpty(itemData.WinLossAmount) && decimal.TryParse(itemData.WinLossAmount, out var winlossAmount) ? winlossAmount : null,
                        SettledDateTime = itemData.SettledDateTime,
                        IpAddress = itemData.IpAddress,
                        UserAgent = itemData.UserAgent,
                        TicketAmount = !string.IsNullOrEmpty(itemData.TicketAmount) && decimal.TryParse(itemData.TicketAmount, out var ticketAmount) ? ticketAmount : null,
                        Status = itemData.Status,
                        TicketCreatedDate = itemData.CreatedDateTime,
                        TicketModifiedDate = itemData.ModifiedDateTime,
                        Selection = itemData.Selection.ToCockFightSelection().ToString(),
                        OddType = itemData.OddsType,
                        ValidStake = !string.IsNullOrEmpty(itemData.ValidStake) && decimal.TryParse(itemData.ValidStake, out var validStake) ? validStake : null,
                        BetKindId = defaultBetKind.Id
                    });
                }
            }

            await cockFightTicketRepos.AddRangeAsync(listAddingItem);
            await LotteryUow.SaveChangesAsync();

            return new Dictionary<string, object>();
        }
    }
}
