using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Enums.Partner.CockFight;
using Lottery.Core.Helpers;
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
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Lottery.Core.Partners.CockFight.GA28
{
    public class CockFightGa28Service : BasePartnerType, ICockFightGa28Service
    {
        private readonly IRedisCacheService _redisCacheService;

        public CockFightGa28Service(ILogger<BasePartnerType> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, IHttpClientFactory httpClientFactory, ILotteryUow lotteryUow, IRedisCacheService redisCacheService) : base(logger, serviceProvider, configuration, clockService, httpClientFactory, lotteryUow)
        {
            _redisCacheService = redisCacheService;
        }

        public override PartnerType PartnerType { get; set; } = PartnerType.GA28;

        public override async Task CreatePlayer(object data)
        {
            if (data is null) return;
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new NotFoundException();
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);

            var ga28CreatePlayerModel = data as Ga28CreateMemberModel;

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
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

            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType) ?? throw new NotFoundException();
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new NotFoundException();
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);

            var httpClient = CreateClient(settingValue.AuthValue);

            var ga28BetSettingData = data as Ga28SyncUpBetSettingModel;

            var url = $"{settingValue.ApiAddress}/api/v1/members/{ga28BetSettingData.MemberRefId.ToLower()}";

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
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new NotFoundException();
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);

            var ga28GenerateUrlModel = data as Ga28LoginPlayerModel;

            var httpClient = CreateClient(settingValue.AuthValue);

            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(new
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

            var objectData = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28LoginPlayerDataReturnModel>(stringData);
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

        public async Task ScanTickets()
        {
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var cockFightTicketRepos = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);
            var fromModifiedDate = settingValue != null && !string.IsNullOrEmpty(settingValue.ScanTicketTime) ? settingValue.ScanTicketTime : DateTime.UtcNow.ToString();
            var toModifiedDate = DateTime.Parse(fromModifiedDate).AddSeconds(30).ToString();

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
            if (response is null) return;

            var stringData = await response.Content.ReadAsStringAsync();
            if (!stringData.IsValidJson()) return;

            Logger.LogInformation($"Login responsed data: {stringData}");

            var listResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ga28RetrieveTicketDataReturnModel>>(stringData);

            // AddNew or Update system tickets
            var listMemberResponseMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => listResponseData.Select(r => r.MemberRefId).Contains(x.MemberRefId)).ToListAsync();
            var players = await playerRepos.FindQueryBy(x => listMemberResponseMapping.Select(p => p.PlayerId).Distinct().Contains(x.PlayerId)).ToListAsync();

            var responseTicketSid = listResponseData.Select(x => x.Sid).ToList();
            var existingTickets = await cockFightTicketRepos.FindQueryBy(x => responseTicketSid.Contains(x.Sid)).ToListAsync();
            var listAddingItem = new List<CockFightTicket>();
            foreach (var data in listResponseData)
            {
                var targetPlayerId = listMemberResponseMapping.FirstOrDefault(x => x.MemberRefId == data.MemberRefId)?.PlayerId;
                var targetPlayer = players.FirstOrDefault(x => x.PlayerId == targetPlayerId);
                if (targetPlayer == null) continue;
                var updatedTicket = existingTickets.FirstOrDefault(x => x.Sid == data.Sid && x.PlayerId == targetPlayer.PlayerId);
                if(updatedTicket != null)
                {
                    // Update existing ticket
                    updatedTicket.AnteAmount = !string.IsNullOrEmpty(data.AnteAmount) && decimal.TryParse(data.AnteAmount, out var anteAmount) ? anteAmount : null;
                    updatedTicket.ArenaCode = data.ArenaCode;
                    updatedTicket.BetAmount = !string.IsNullOrEmpty(data.BetAmount) && decimal.TryParse(data.BetAmount, out var betAmount) ? betAmount : null;
                    updatedTicket.CurrencyCode = data.CurrencyCode;
                    updatedTicket.FightNumber = data.FightNumber;
                    updatedTicket.MatchDayCode = data.MatchDayCode;
                    updatedTicket.Odds = !string.IsNullOrEmpty(data.Odds) && decimal.TryParse(data.Odds, out var odds) ? odds : null;
                    updatedTicket.Result = ConvertTicketResult(data.Result);
                    updatedTicket.WinlossAmount = !string.IsNullOrEmpty(data.WinLossAmount) && decimal.TryParse(data.WinLossAmount, out var winlossAmount) ? winlossAmount : null;
                    updatedTicket.SettledDateTime = data.SettledDateTime;
                    updatedTicket.IpAddress = data.IpAddress;
                    updatedTicket.UserAgent = data.UserAgent;
                    updatedTicket.TicketAmount = !string.IsNullOrEmpty(data.TicketAmount) && decimal.TryParse(data.TicketAmount, out var ticketAmount) ? ticketAmount : null;
                    updatedTicket.Status = data.Status;
                    updatedTicket.TicketModifiedDate = data.ModifiedDateTime;
                    updatedTicket.Selection = data.Selection;
                    updatedTicket.OddType = data.OddsType;
                    updatedTicket.ValidStake = !string.IsNullOrEmpty(data.ValidStake) && decimal.TryParse(data.ValidStake, out var validStake) ? validStake : null;
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
                        Sid = data.Sid,
                        AnteAmount = !string.IsNullOrEmpty(data.AnteAmount) && decimal.TryParse(data.AnteAmount, out var anteAmount) ? anteAmount : null,
                        ArenaCode = data.ArenaCode,
                        BetAmount = !string.IsNullOrEmpty(data.BetAmount) && decimal.TryParse(data.BetAmount, out var betAmount) ? betAmount : null,
                        CurrencyCode = data.CurrencyCode,
                        FightNumber = data.FightNumber,
                        MatchDayCode = data.MatchDayCode,
                        Odds = !string.IsNullOrEmpty(data.Odds) && decimal.TryParse(data.Odds, out var odds) ? odds : null,
                        Result = ConvertTicketResult(data.Result),
                        WinlossAmount = !string.IsNullOrEmpty(data.WinLossAmount) && decimal.TryParse(data.WinLossAmount, out var winlossAmount) ? winlossAmount : null,
                        SettledDateTime = data.SettledDateTime,
                        IpAddress = data.IpAddress,
                        UserAgent = data.UserAgent,
                        TicketAmount = !string.IsNullOrEmpty(data.TicketAmount) && decimal.TryParse(data.TicketAmount, out var ticketAmount) ? ticketAmount : null,
                        Status = data.Status,
                        TicketCreatedDate = data.CreatedDateTime,
                        TicketModifiedDate = data.ModifiedDateTime,
                        Selection = data.Selection,
                        OddType = data.OddsType,
                        ValidStake = !string.IsNullOrEmpty(data.ValidStake) && decimal.TryParse(data.ValidStake, out var validStake) ? validStake : null,
                        BetKindId = 1 // need to check this later
                    });
                }
            }

            await cockFightTicketRepos.AddRangeAsync(listAddingItem);
            await LotteryUow.SaveChangesAsync();
        }

        private int ConvertTicketResult(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;
            return data switch
            {
                "win" => CockFightTicketResult.Win.ToInt(),
                "loss" => CockFightTicketResult.Loss.ToInt(),
                "draw" => CockFightTicketResult.Draw.ToInt(),
                _ => default,
            };
        }
    }
}
