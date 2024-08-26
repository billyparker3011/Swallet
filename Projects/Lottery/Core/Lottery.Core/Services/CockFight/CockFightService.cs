using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Enums.Partner;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetBalance;
using Lottery.Core.Models.CockFight.UpdateCockFightBookieSetting;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Ga28;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Wallet;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightService : LotteryBaseService<CockFightService>, ICockFightService
    {
        private readonly IPartnerPublishService _partnerPublishService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ISingleWalletService _singleWalletService;

        public CockFightService(
            ILogger<CockFightService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow,
            IPartnerPublishService partnerPublishService,
            IRedisCacheService redisCacheService,
            ISingleWalletService singleWalletService)
            : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _partnerPublishService = partnerPublishService;
            _redisCacheService = redisCacheService;
            _singleWalletService = singleWalletService;
        }

        public async Task CreateCockFightPlayer()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightPlayerBetSetting = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);
            var accountId = settingValue?.PartnerAccountId ?? throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            // Create CockFight player
            bool isSuccessCreatedPlayer = true;
            var generatedMemberRefId = Guid.NewGuid().ToString();
            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial)
            {
                try
                {
                    await _partnerPublishService.Publish(new Ga28CreateMemberModel
                    {
                        Partner = PartnerType.GA28,
                        PlayerId = ClientContext.Player.PlayerId,
                        AccountId = accountId,
                        MemberRefId = generatedMemberRefId
                    });
                }
                catch (Exception ex)
                {
                    isSuccessCreatedPlayer = false;
                    Logger.LogError($"Create cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
                }

                if (!isSuccessCreatedPlayer) return;
                await cockFightPlayerMappingRepos.AddAsync(new Data.Entities.Partners.CockFight.CockFightPlayerMapping
                {
                    PlayerId = ClientContext.Player.PlayerId,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId,
                    IsInitial = true,
                    IsEnabled = true,
                    IsFreeze = false,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0
                });
            }

            // Sync up player bet setting
            var playerBetSetting = await cockFightPlayerBetSetting.FindBetSettingByPlayerId(ClientContext.Player.PlayerId);
            try
            {
                await _partnerPublishService.Publish(new Ga28SyncUpBetSettingModel
                {
                    Partner = PartnerType.GA28,
                    MainLimitAmountPerFight = playerBetSetting?.MainLimitAmountPerFight,
                    DrawLimitAmountPerFight = playerBetSetting?.DrawLimitAmountPerFight,
                    LimitNumTicketPerFight = playerBetSetting?.LimitNumTicketPerFight,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Update bet setting cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
            }

            await LotteryUow.SaveChangesAsync();
        }

        public async Task<GetCockFightPlayerBalanceResult> GetCockFightPlayerBalance(string memberRefId)
        {
            var cockFightPlayerMapping = await GetMappingPlayer(memberRefId);
            var balance = 0m;
            if (cockFightPlayerMapping != null) balance = await _singleWalletService.GetBalanceForGa28(cockFightPlayerMapping.PlayerId);
            return new GetCockFightPlayerBalanceResult { Balance = balance.ToString() };
        }

        private async Task<CockFightPlayerMapping> GetMappingPlayer(string memberRefId)
        {
            var cockFightPlayerMappingRepository = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            return await cockFightPlayerMappingRepository.FindByMemberRefId(memberRefId);
        }

        private async Task<Data.Entities.Player> GetPlayer(long playerId)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            return await playerRepository.FindByIdAsync(playerId);
        }

        public async Task<LoginPlayerInformationDto> GetCockFightUrl()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial) return null;

            var clientUrlKey = ClientContext.Player.PlayerId.GetGa28ClientUrlByPlayerId();
            var clientUrlHash = await _redisCacheService.HashGetFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (!clientUrlHash.TryGetValue(clientUrlKey.SubKey, out string gameUrl)) return null;
            return new LoginPlayerInformationDto
            {
                GameClientUrl = gameUrl
            };
        }

        public async Task LoginCockFightPlayer()
        {
            var cockFightPlayerMappingRepos = LotteryUow.GetRepository<ICockFightPlayerMappingRepository>();
            var bookiesSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var cockFightPlayerBetSetting = LotteryUow.GetRepository<ICockFightPlayerBetSettingRepository>();

            var cockFightPlayerMapping = await cockFightPlayerMappingRepos.FindQueryBy(x => x.PlayerId == ClientContext.Player.PlayerId).FirstOrDefaultAsync();

            var cockFightRequestSetting = await bookiesSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);
            if (string.IsNullOrEmpty(cockFightRequestSetting.SettingValue)) throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);
            var settingValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightRequestSetting.SettingValue);
            var accountId = settingValue?.PartnerAccountId ?? throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            // Create CockFight player mapping
            bool isSyncBetSetting = false;
            var generatedMemberRefId = Guid.NewGuid().ToString();
            if (cockFightPlayerMapping == null || !cockFightPlayerMapping.IsInitial)
            {
                isSyncBetSetting = true;
                await cockFightPlayerMappingRepos.AddAsync(new Data.Entities.Partners.CockFight.CockFightPlayerMapping
                {
                    PlayerId = ClientContext.Player.PlayerId,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId,
                    IsInitial = true,
                    IsEnabled = true,
                    IsFreeze = false,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0
                });
            }
            else
            {
                generatedMemberRefId = cockFightPlayerMapping.MemberRefId;
            }

            // Remove existing token
            var clientUrlKey = ClientContext.Player.PlayerId.GetGa28ClientUrlByPlayerId();
            await _redisCacheService.HashDeleteFieldsAsync(clientUrlKey.MainKey, new List<string> { clientUrlKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            var tokenKey = ClientContext.Player.PlayerId.GetGa28TokenByPlayerId();
            await _redisCacheService.HashDeleteFieldsAsync(tokenKey.MainKey, new List<string> { tokenKey.SubKey }, CachingConfigs.RedisConnectionForApp);

            await LotteryUow.SaveChangesAsync();

            // Login and save game url to redis cache
            try
            {
                await _partnerPublishService.Publish(new Ga28LoginPlayerModel
                {
                    Partner = PartnerType.GA28,
                    AccountId = accountId,
                    MemberRefId = generatedMemberRefId
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Login cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
            }

            // Sync up player bet setting
            if (isSyncBetSetting)
            {
                var playerBetSetting = await cockFightPlayerBetSetting.FindBetSettingByPlayerId(ClientContext.Player.PlayerId);
                try
                {
                    await _partnerPublishService.Publish(new Ga28SyncUpBetSettingModel
                    {
                        Partner = PartnerType.GA28,
                        MainLimitAmountPerFight = playerBetSetting?.MainLimitAmountPerFight,
                        DrawLimitAmountPerFight = playerBetSetting?.DrawLimitAmountPerFight,
                        LimitNumTicketPerFight = playerBetSetting?.LimitNumTicketPerFight,
                        AccountId = accountId,
                        MemberRefId = generatedMemberRefId
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Update bet setting cock fight player with id = {ClientContext.Player.PlayerId} failed. Detail : {ex}");
                }
            }
        }

        public async Task TransferCockFightPlayerTickets(Ga28TransferTicketModel model)
        {
            var cockFightPlayerMapping = await GetMappingPlayer(model.MemberRefId.ToString());
            if (cockFightPlayerMapping == null) return;

            var player = await GetPlayer(cockFightPlayerMapping.PlayerId);
            if (player == null) return;

            if (string.IsNullOrEmpty(model.Ticket.TicketAmount) || !decimal.TryParse(model.Ticket.TicketAmount, out decimal ticketAmount)) return;

            var balance = await _singleWalletService.GetBalanceForGa28(player.PlayerId);
            if (ticketAmount > balance) throw new HnMicroException();

            var betKindRepository = LotteryUow.GetRepository<ICockFightBetKindRepository>();
            var betKind = await betKindRepository.FindQueryBy(f => true).FirstOrDefaultAsync();
            if (betKind == null) return;

            //  TODO Need to storage Match & Fight
            var cockFightTicketRepository = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var ticket = await cockFightTicketRepository.FindBySId(model.Ticket.Sid);
            if (ticket == null)
            {
                ticket = new CockFightTicket
                {
                    PlayerId = player.PlayerId,
                    AgentId = player.AgentId,
                    MasterId = player.MasterId,
                    SupermasterId = player.SupermasterId,
                    BetKindId = betKind.Id,
                    AnteAmount = string.IsNullOrEmpty(model.Ticket.AnteAmount) || !decimal.TryParse(model.Ticket.AnteAmount, out decimal anteAmount) ? null : anteAmount,
                    ArenaCode = model.Ticket.ArenaCode,
                    BetAmount = string.IsNullOrEmpty(model.Ticket.BetAmount) || !decimal.TryParse(model.Ticket.BetAmount, out decimal betAmount) ? null : betAmount,
                    CurrencyCode = model.Ticket.CurrencyCode,
                    FightNumber = model.Ticket.FightNumber,
                    MatchDayCode = model.Ticket.MatchDayCode,
                    Odds = string.IsNullOrEmpty(model.Ticket.Odds) || !decimal.TryParse(model.Ticket.Odds, out decimal odds) ? null : odds,
                    Result = model.Ticket.Result.ToTicketResult(),
                    Selection = model.Ticket.Selection.ToCockFightSelection().ToString(),
                    SettledDateTime = model.Ticket.SettledDateTime,
                    Sid = model.Ticket.Sid,
                    Status = model.Ticket.Status,
                    TicketAmount = ticketAmount,
                    WinlossAmount = string.IsNullOrEmpty(model.Ticket.WinLossAmount) || !decimal.TryParse(model.Ticket.WinLossAmount, out decimal winlossAmount) ? null : winlossAmount,
                    IpAddress = model.Ticket.IpAddress,
                    UserAgent = model.Ticket.UserAgent,
                    TicketCreatedDate = model.Ticket.CreatedDateTime,
                    TicketModifiedDate = model.Ticket.ModifiedDateTime,
                    ValidStake = string.IsNullOrEmpty(model.Ticket.ValidStake) || !decimal.TryParse(model.Ticket.ValidStake, out decimal validStake) ? null : validStake,
                    OddsType = model.Ticket.OddsType,
                    CreatedAt = model.Ticket.CreatedDateTime,
                    CreatedBy = player.PlayerId
                };
                cockFightTicketRepository.Add(ticket);
            }
            else
            {
                ticket.Result = model.Ticket.Result.ToTicketResult();
                ticket.SettledDateTime = model.Ticket.SettledDateTime;
                ticket.Status = model.Ticket.Status;
                ticket.WinlossAmount = string.IsNullOrEmpty(model.Ticket.WinLossAmount) || !decimal.TryParse(model.Ticket.WinLossAmount, out decimal winlossAmount) ? null : winlossAmount;
                ticket.TicketModifiedDate = model.Ticket.ModifiedDateTime;
                ticket.UpdatedAt = model.Ticket.ModifiedDateTime;
                ticket.UpdatedBy = player.PlayerId;
                cockFightTicketRepository.Update(ticket);
            }

            await LotteryUow.SaveChangesAsync();
        }

        public async Task UpdateCockFightBookieSetting(UpdateCockFightBookieSettingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var bookieSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var agent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (agent.RoleId != Role.Company.ToInt()) return;

            var cockFightBookieSetting = await bookieSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);

            if (cockFightBookieSetting.SettingValue == null) throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            var setting = JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightBookieSetting.SettingValue);
            if (setting == null) return;

            setting.AuthValue = model.AuthValue;
            setting.ApiAddress = model.ApiAddress;
            setting.AllowScanByRange = model.AllowScanByRange;
            setting.FromScanByRange = model.FromScanByRange;
            setting.ToScanByRange = model.ToScanByRange;
            setting.GameClientId = model.GameClientId;
            setting.PartnerAccountId = model.PartnerAccountId;
            setting.PrivateKey = model.PrivateKey;
            setting.IsMaintainance = model.IsMaintainance;

            cockFightBookieSetting.SettingValue = JsonConvert.SerializeObject(setting);
            bookieSettingRepos.Update(cockFightBookieSetting);
            await LotteryUow.SaveChangesAsync();
        }

        public async Task<UpdateCockFightBookieSettingModel> GetCockFightBookieSetting()
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var bookieSettingRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();

            var agent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (agent.RoleId != Role.Company.ToInt()) return null;
            var cockFightBookieSetting = await bookieSettingRepos.FindBookieSettingByType(PartnerType.GA28) ?? throw new BadRequestException(ErrorCodeHelper.CockFight.BookieSettingIsNotBeingInitiated);

            if (cockFightBookieSetting.SettingValue == null) throw new BadRequestException(ErrorCodeHelper.CockFight.PartnerAccountIdHasNotBeenProvided);

            var setting = JsonConvert.DeserializeObject<Ga28BookieSettingValue>(cockFightBookieSetting.SettingValue);
            if (setting == null) return new UpdateCockFightBookieSettingModel();

            return new UpdateCockFightBookieSettingModel
            {
                AllowScanByRange = setting.AllowScanByRange,
                ApiAddress = setting.ApiAddress,
                AuthValue = setting.AuthValue,
                FromScanByRange = setting.FromScanByRange,
                GameClientId = setting.GameClientId,
                IsMaintainance = setting.IsMaintainance ?? false,
                PartnerAccountId = setting.PartnerAccountId,
                PrivateKey = setting.PrivateKey,
                ToScanByRange = setting.ToScanByRange
            };
        }
    }
}
