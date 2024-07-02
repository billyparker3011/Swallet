using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentBetSettings;
using Lottery.Core.Models.Agent.GetCreditBalanceDetailPopup;
using Lottery.Core.Models.Player;
using Lottery.Core.Models.Player.CreatePlayer;
using Lottery.Core.Models.Player.UpdatePlayer;
using Lottery.Core.Models.Player.UpdatePlayerCreditBalance;
using Lottery.Core.Models.Setting;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Audit;
using Lottery.Core.Services.Authentication;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class PlayerService : LotteryBaseService<PlayerService>, IPlayerService
    {
        private readonly ISessionService _sessionService;
        private readonly IProcessTicketService _processTicketService;
        private readonly IPlayerSettingService _playerSettingService;
        private readonly IAuditService _auditService;

        public PlayerService(ILogger<PlayerService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            ISessionService sessionService,
            IProcessTicketService processTicketService,
            IPlayerSettingService playerSettingService,
            IAuditService auditService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _sessionService = sessionService;
            _processTicketService = processTicketService;
            _playerSettingService = playerSettingService;
            _auditService = auditService;
        }

        public async Task CreatePlayer(CreatePlayerModel model)
        {
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerOddRepos = LotteryUow.GetRepository<IPlayerOddsRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId != (int)Role.Agent) return;
            var passwordDecoded = model.Password.DecodePassword();

            await ValidatePlayerUsernamePassword(playerRepos, model.Username, passwordDecoded);

            await ValidatePlayerCredit(playerRepos, model, clientAgent);

            var createdAt = ClockService.GetUtcNow();

            var player = new Data.Entities.Player
            {
                Username = model.Username.ToUpper(),
                Password = passwordDecoded.Md5(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Credit = model.Credit,
                Lock = false,

                SupermasterId = clientAgent.SupermasterId,
                MasterId = clientAgent.MasterId,
                AgentId = clientAgent.AgentId,
                State = UserState.Open.ToInt(),

                CreatedAt = createdAt,
                CreatedBy = clientAgent.AgentId
            };
            await playerRepos.AddAsync(player);

            //Add bet settings
            var playerBetSettings = model.BetSettings.Select(x => new PlayerOdd
            {
                Player = player,
                BetKindId = x.BetKindId,
                MinBet = x.ActualMinBet,
                MaxBet = x.ActualMaxBet,
                Buy = x.ActualBuy,
                MaxPerNumber = x.ActualMaxPerNumber,
                CreatedAt = createdAt,
                CreatedBy = clientAgent.AgentId
            }).ToList();
            await playerOddRepos.AddRangeAsync(playerBetSettings);

            await LotteryUow.SaveChangesAsync();

            var dictBetSettings = new Dictionary<int, BetSettingModel>();
            playerBetSettings.ForEach(f =>
            {
                dictBetSettings[f.BetKindId] = new BetSettingModel
                {
                    MinBet = f.MinBet,
                    MaxBet = f.MaxBet,
                    MaxPerNumber = f.MaxPerNumber,
                    OddsValue = f.Buy
                };
            });
            //  Process cache
            await _processTicketService.BuildGivenCreditCache(player.PlayerId, player.Credit);
            await _playerSettingService.BuildSettingByBetKindCache(player.PlayerId, dictBetSettings);

            await _auditService.SaveAuditData(new AuditParams
            {
                Type = (int)AuditType.Credit,
                EditedUsername = ClientContext.Agent.UserName,
                AgentUserName = player.Username,
                AgentFirstName = player.FirstName,
                AgentLastName = player.LastName,
                Action = AuditDataHelper.Credit.Action.ActionSetCreditWhenUserCreated,
                DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailSetCreditWhenUserCreated, player.Username, string.Empty, ClientContext.Agent.ParentId != 0 ? ClientContext.Agent.UserName : string.Empty),
                OldValue = 0m,
                NewValue = player.Credit,
                SupermasterId = player.SupermasterId,
                MasterId = player.MasterId,
                AgentId = player.AgentId
            });
        }

        public async Task UpdatePlayer(UpdatePlayerModel model)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var updatedPlayer = await playerRepository.FindByIdAsync(model.PlayerId) ?? throw new NotFoundException();

            updatedPlayer.FirstName = model.FirstName ?? updatedPlayer.FirstName;
            updatedPlayer.LastName = model.LastName ?? updatedPlayer.LastName;
            var oldStateValue = updatedPlayer.State;
            updatedPlayer.State = model.State.HasValue && model.State.Value != UserState.All.ToInt() ? model.State.Value : updatedPlayer.State;

            updatedPlayer.UpdatedAt = ClockService.GetUtcNow();
            updatedPlayer.UpdatedBy = ClientContext.Agent.AgentId;

            var oldCreditValue = updatedPlayer.Credit;
            updatedPlayer.Credit = model.Credit ?? updatedPlayer.Credit;
            playerRepository.Update(updatedPlayer);

            await LotteryUow.SaveChangesAsync();

            if (model.Credit.HasValue)
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Credit,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = updatedPlayer.Username,
                    Action = AuditDataHelper.Credit.Action.ActionUpdateGivenCredit,
                    DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailUpdateGivenCredit, updatedPlayer.Username),
                    OldValue = oldCreditValue,
                    NewValue = updatedPlayer.Credit,
                    SupermasterId = updatedPlayer.SupermasterId,
                    MasterId = updatedPlayer.MasterId,
                    AgentId = updatedPlayer.AgentId
                });
            }

            if (model.State.HasValue && model.State.Value != UserState.All.ToInt())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.State,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = updatedPlayer.Username,
                    Action = AuditDataHelper.State.Action.ActionUpdateState,
                    DetailMessage = string.Format(AuditDataHelper.State.DetailMessage.DetailUpdateState, updatedPlayer.Username, Enum.GetName(typeof(UserState), oldStateValue), Enum.GetName(typeof(UserState), updatedPlayer.State)),
                    OldValue = oldStateValue,
                    NewValue = updatedPlayer.State,
                    SupermasterId = updatedPlayer.SupermasterId,
                    MasterId = updatedPlayer.MasterId,
                    AgentId= updatedPlayer.AgentId
                });
            }

            await _processTicketService.BuildGivenCreditCache(updatedPlayer.PlayerId, updatedPlayer.Credit);
        }

        private async Task ValidatePlayerUsernamePassword(IPlayerRepository playerRepos, string username, string passwordDecoded)
        {
            var existedUsername = await playerRepos.FindByUsernameAndPassword(username, passwordDecoded.Md5());
            var isEnoughComlex = passwordDecoded.IsStrongPassword();
            if (existedUsername != null)
                throw new BadRequestException(ErrorCodeHelper.Agent.UsernameIsExist);
            if (!isEnoughComlex)
                throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
        }

        private async Task ValidatePlayerCredit(IPlayerRepository playerRepos, CreatePlayerModel model, Data.Entities.Agent clientAgent)
        {
            if (clientAgent.RoleId == (int)Role.Company) return;
            if (clientAgent.MemberMaxCredit.HasValue && model.Credit > clientAgent.MemberMaxCredit.Value)
                throw new BadRequestException(ErrorCodeHelper.Agent.CreditOverLimitation);
            var totalValidCredit = clientAgent.Credit;
            var totalCreditUsed = await playerRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId).SumAsync(x => x.Credit);
            if ((totalCreditUsed + model.Credit) > totalValidCredit)
                throw new BadRequestException(ErrorCodeHelper.Agent.InvalidCredit);
        }

        public async Task Logout()
        {
            var playerSessionRepository = LotteryUow.GetRepository<IPlayerSessionRepository>();
            var playerSession = await playerSessionRepository.FindByPlayerId(ClientContext.Player.PlayerId) ?? throw new NotFoundException();
            playerSession.Hash = string.Empty;
            playerSession.State = SessionState.Offline.ToInt();
            await LotteryUow.SaveChangesAsync();

            await _sessionService.RemoveSession(ClientContext.Player.RoleId, ClientContext.Player.PlayerId);
        }

        public async Task<string> GetSuggestionPlayerIdentifier()
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var players = await playerRepository.FindQueryBy(f => f.AgentId == (ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId)).ToListAsync();
            var usernameIdentifies = players.Select(x => x.Username.Substring(x.Username.Length - 3, 3)).ToList();
            return usernameIdentifies.GetNextTripleCharacters();
        }

        public async Task UpdatePlayerBetSetting(long playerId, List<AgentBetSettingDto> updateItems)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var player = await playerRepository.FindByIdAsync(playerId) ?? throw new NotFoundException();

            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();

            var auditBetSettings = new List<AuditSettingData>();
            var updateBetKindIds = updateItems.Select(x => x.BetKindId);
            var updatedBetKinds = await betKindRepos.FindQueryBy(x => updateBetKindIds.Contains(x.Id)).ToListAsync();
            var existedPlayerBetSettings = await playerOddRepository.FindQueryBy(x => x.PlayerId == player.PlayerId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();

            var dictBetSettings = new Dictionary<int, BetSettingModel>();
            existedPlayerBetSettings.ForEach(item =>
            {
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if (updateItem != null)
                {
                    var oldMinBetValue = item.MinBet;
                    var oldMaxBetValue = item.MaxBet;
                    var oldMaxPerNumber = item.MaxPerNumber;
                    item.MinBet = updateItem.ActualMinBet;
                    item.MaxBet = updateItem.ActualMaxBet;
                    item.MaxPerNumber = updateItem.ActualMaxPerNumber;

                    dictBetSettings[item.BetKindId] = new BetSettingModel
                    {
                        MinBet = updateItem.ActualMinBet,
                        MaxBet = updateItem.ActualMaxBet,
                        MaxPerNumber = updateItem.ActualMaxPerNumber,
                        OddsValue = item.Buy
                    };

                    auditBetSettings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MinBetTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMinBetValue,
                            NewValue = item.MinBet
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MaxBetTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMaxBetValue,
                            NewValue = item.MaxBet
                        },
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.MaxPerNumberTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldMaxPerNumber,
                            NewValue = item.MaxPerNumber
                        }
                    });
                }
            });
            await LotteryUow.SaveChangesAsync();

            if (existedPlayerBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = player.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = player.SupermasterId,
                    MasterId = player.MasterId,
                    AgentId = player.AgentId,
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }

            //  Process cache
            await _playerSettingService.BuildSettingByBetKindCache(player.PlayerId, dictBetSettings);
        }

        public async Task UpdatePlayerCreditBalance(UpdatePlayerCreditBalanceModel updateItem)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var updatedPlayer = await playerRepos.FindByIdAsync(updateItem.PlayerId) ?? throw new NotFoundException();

            decimal outstanding = 0m;
            var agent = await agentRepos.FindByIdAsync(updatedPlayer.AgentId) ?? throw new NotFoundException();
            var availableCreditOfAgent = await playerRepos.FindQueryBy(x => x.AgentId == agent.AgentId).SumAsync(x => x.Credit);
            var maxCreditToCompare = agent.MemberMaxCredit.HasValue ? Math.Min(agent.MemberMaxCredit.Value, agent.Credit - outstanding - availableCreditOfAgent) : agent.Credit - outstanding - availableCreditOfAgent;
            if (updateItem.Credit < outstanding || updateItem.Credit > maxCreditToCompare)
                throw new BadRequestException(ErrorCodeHelper.Agent.InvalidCredit);

            var oldCreditValue = updatedPlayer.Credit;
            updatedPlayer.Credit = updateItem.Credit;
            updatedPlayer.UpdatedAt = ClockService.GetUtcNow();
            updatedPlayer.UpdatedBy = ClientContext.Agent.AgentId;
            playerRepos.Update(updatedPlayer);

            await LotteryUow.SaveChangesAsync();

            await _auditService.SaveAuditData(new AuditParams
            {
                Type = (int)AuditType.Credit,
                EditedUsername = ClientContext.Agent.UserName,
                AgentUserName = updatedPlayer.Username,
                AgentFirstName = updatedPlayer.FirstName,
                AgentLastName = updatedPlayer.LastName,
                Action = AuditDataHelper.Credit.Action.ActionUpdatePlayerCredit,
                DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailUpdatePlayerCredit, updatedPlayer.Username),
                OldValue = oldCreditValue,
                NewValue = updatedPlayer.Credit,
                SupermasterId = updatedPlayer.SupermasterId,
                MasterId = updatedPlayer.MasterId,
                AgentId = updatedPlayer.AgentId
            });

            await _processTicketService.BuildGivenCreditCache(updatedPlayer.PlayerId, updatedPlayer.Credit);
        }

        public async Task<GetCreditBalanceDetailPopupResult> GetCreditBalanceDetailPopup(long playerId)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var targetPlayer = await playerRepos.FindByIdAsync(playerId) ?? throw new NotFoundException();

            decimal outstanding = 0m; //TODO: update later
            var agent = await agentRepos.FindByIdAsync(targetPlayer.AgentId) ?? throw new NotFoundException();
            var availableCreditOfAgent = await playerRepos.FindQueryBy(x => x.AgentId == agent.AgentId).SumAsync(x => x.Credit);
            
            var maxCreditToCompare = agent.MemberMaxCredit.HasValue ? Math.Min(agent.MemberMaxCredit.Value, agent.Credit - outstanding - availableCreditOfAgent) : agent.Credit - outstanding - availableCreditOfAgent;

            return new GetCreditBalanceDetailPopupResult
            {
                CurrentGivenCredit = targetPlayer.Credit,
                GivenCredit = targetPlayer.Credit,
                MinCredit = outstanding,
                MaxCredit = maxCreditToCompare
            };
        }

        public async Task<GetAgentBetSettingsResult> GetDetailPlayerBetSettings(long playerId)
        {
            //Init repos
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var playerOddRepos = LotteryUow.GetRepository<IPlayerOddsRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddsRepository>();

            var targetPlayer = await playerRepos.FindByIdAsync(playerId) ?? throw new NotFoundException();

            var companyOdds = await agentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Company.ToInt() && x.Agent.ParentId == 0L).ToListAsync();
            var defaultBetSettings = await agentOddRepos.FindQuery().Include(x => x.Agent).Where(x => x.Agent.RoleId == Role.Agent.ToInt() && x.AgentId == targetPlayer.AgentId).ToListAsync();

            var playerBetSettings = await playerOddRepos.FindQuery()
                                                        .Include(x => x.Player)
                                                        .Include(x => x.BetKind)
                                                        .Where(x => x.Player.PlayerId == targetPlayer.PlayerId
                                                                    && x.BetKind.Id != 1000)
                                                        .OrderBy(x => x.BetKind.RegionId)
                                                        .ThenBy(x => x.BetKind.CategoryId)
                                                        .ThenBy(x => x.BetKind.OrderInCategory)
                                                        .Select(x => new AgentBetSettingDto
                                                        {
                                                            BetKindId = x.BetKind.Id,
                                                            RegionId = x.BetKind.RegionId,
                                                            CategoryId = x.BetKind.CategoryId,
                                                            BetKindName = x.BetKind.Name,
                                                            MinBuy = x.Buy,
                                                            ActualBuy = x.Buy,
                                                            DefaultMinBet = x.MinBet,
                                                            ActualMinBet = x.MinBet,
                                                            DefaultMaxBet = x.MaxBet,
                                                            ActualMaxBet = x.MaxBet,
                                                            DefaultMaxPerNumber = x.MaxPerNumber,
                                                            ActualMaxPerNumber = x.MaxPerNumber,
                                                        })
                                                        .ToListAsync();
            foreach (var item in playerBetSettings)
            {
                var maxBuy = companyOdds.FirstOrDefault(x => x.BetKindId == item.BetKindId)?.MaxBuy;
                var defaultBetKindItem = defaultBetSettings.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                item.DefaultMinBet = defaultBetKindItem != null ? defaultBetKindItem.MinBet : item.DefaultMinBet;
                item.DefaultMaxBet = defaultBetKindItem != null ? defaultBetKindItem.MaxBet : item.DefaultMaxBet;
                item.DefaultMaxPerNumber = defaultBetKindItem != null ? defaultBetKindItem.MaxPerNumber : item.DefaultMaxPerNumber;
                item.MinBuy = defaultBetKindItem.Buy;
                item.MaxBuy = maxBuy ?? decimal.Zero;
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return new GetAgentBetSettingsResult
            {
                AgentBetSettings = playerBetSettings
            };
        }

        public async Task<PlayerModel> GetPlayer(long playerId)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(playerId);
            if (player == null) return null;
            return new PlayerModel
            {
                Id = playerId,
                Username = player.Username,
                RoleId = Role.Player,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Lock = player.Lock,
                State = player.State.ToEnum<UserState>(),
                ParentState = player.ParentState.HasValue ? player.ParentState.Value.ToEnum<UserState>() : null
            };
        }
    }
}
