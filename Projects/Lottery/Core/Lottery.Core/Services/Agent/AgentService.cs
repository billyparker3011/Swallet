using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.Audit;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.CreateAgent;
using Lottery.Core.Models.Agent.CreateSubAgent;
using Lottery.Core.Models.Agent.GetAgentCreditBalance;
using Lottery.Core.Models.Agent.GetAgentCreditInfo;
using Lottery.Core.Models.Agent.GetAgentDashBoard;
using Lottery.Core.Models.Agent.GetAgents;
using Lottery.Core.Models.Agent.GetAgentWinLossSummary;
using Lottery.Core.Models.Agent.GetCreditBalanceDetailPopup;
using Lottery.Core.Models.Agent.GetSubAgents;
using Lottery.Core.Models.Agent.UpdateAgent;
using Lottery.Core.Models.Agent.UpdateAgentCreditBalance;
using Lottery.Core.Models.Setting;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Audit;
using Lottery.Core.Services.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Expressions;

namespace Lottery.Core.Services.Agent
{
    public class AgentService : LotteryBaseService<AgentService>, IAgentService
    {
        private readonly IAuditService _auditService;
        private readonly IPlayerSettingService _playerSettingService;
        private readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("de-DE");

        public AgentService(ILogger<AgentService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IAuditService auditService,
            IPlayerSettingService playerSettingService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _auditService = auditService;
            _playerSettingService = playerSettingService;
        }

        public async Task CreateAgent(CreateAgentModel model)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var agentOddRepos = LotteryUow.GetRepository<IAgentOddsRepository>();
            var agentPositionTakingRepos = LotteryUow.GetRepository<IAgentPositionTakingRepository>();

            var targetAgentId = ClientContext.Agent.ParentId == 0
                                ? ClientContext.Agent.AgentId
                                : ClientContext.Agent.ParentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var passwordDecoded = model.Password.DecodePassword();

            await ValidateAgentUsernamePassword(agentRepos, model.Username, passwordDecoded);

            await ValidateAgentCredit(agentRepos, model, clientAgent);

            var createdAt = ClockService.GetUtcNow();

            var newAgent = new Data.Entities.Agent
            {
                Username = model.Username.ToUpper(),
                Password = passwordDecoded.Md5(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Credit = model.Credit,
                MemberMaxCredit = model.MemberMaxCredit,
                RoleId = clientAgent.RoleId + 1,
                Permissions = AgentHelper.DefaultPermissions,
                State = UserState.Open.ToInt(),
                SupermasterId = GetSuperMasterId(clientAgent),
                MasterId = GetMasterId(clientAgent),
                ParentId = 0,
                ParentState = clientAgent.State,
                CreatedAt = createdAt
            };
            await agentRepos.AddAsync(newAgent);

            //Add bet settings
            var agentBetSettings = model.BetSettings.Select(x => new AgentOdd
            {
                Agent = newAgent,
                BetKindId = x.BetKindId,
                MinBet = x.ActualMinBet,
                MaxBet = x.ActualMaxBet,
                Buy = x.ActualBuy,
                MinBuy = x.MinBuy,
                MaxBuy = x.MaxBuy,
                MaxPerNumber = x.ActualMaxPerNumber,
                CreatedAt = createdAt
            }).ToList();
            await agentOddRepos.AddRangeAsync(agentBetSettings);

            //Add position takings
            var agentPositionTakings = model.PositionTakings.Select(x => new AgentPositionTaking
            {
                Agent = newAgent,
                BetKindId = x.BetKindId,
                PositionTaking = x.ActualPositionTaking,
                CreatedAt = createdAt
            }).ToList();
            await agentPositionTakingRepos.AddRangeAsync(agentPositionTakings);

            await LotteryUow.SaveChangesAsync();

            await _auditService.SaveAuditData(new AuditParams
            {
                Type = (int)AuditType.Credit,
                EditedUsername = ClientContext.Agent.UserName,
                AgentUserName = newAgent.Username,
                AgentFirstName = newAgent.FirstName,
                AgentLastName = newAgent.LastName,
                Action = AuditDataHelper.Credit.Action.ActionSetCreditWhenUserCreated,
                DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailSetCreditWhenUserCreated, newAgent.Username, newAgent.MemberMaxCredit?.ToString("N3", _culture), ClientContext.Agent.ParentId != 0 ? ClientContext.Agent.UserName : string.Empty),
                OldValue = 0m,
                NewValue = newAgent.Credit,
                SupermasterId = GetAuditSupermasterId(newAgent),
                MasterId = GetAuditMasterId(newAgent)
            });
        }

        private long GetAuditMasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Agent ? targetUser.MasterId : 0;
        }

        private long GetAuditSupermasterId(Data.Entities.Agent targetUser)
        {
            return targetUser.RoleId is (int)Role.Master or (int)Role.Agent ? targetUser.SupermasterId : 0;
        }

        private long GetMasterId(Data.Entities.Agent clientAgent)
        {
            if (clientAgent.RoleId == (int)Role.Company)
            {
                return 0;
            }
            return clientAgent.RoleId is (int)Role.Master ? clientAgent.AgentId : clientAgent.MasterId;
        }

        private long GetSuperMasterId(Data.Entities.Agent clientAgent)
        {
            if (clientAgent.RoleId == (int)Role.Company)
            {
                return 0;
            }
            return clientAgent.RoleId is (int)Role.Supermaster ? clientAgent.AgentId : clientAgent.SupermasterId;
        }

        private async Task ValidateAgentUsernamePassword(IAgentRepository agentRepos, string username, string passwordDecoded)
        {
            var existedUsername = await agentRepos.FindByUsernamePassword(username, passwordDecoded.Md5());
            var isEnoughComlex = passwordDecoded.IsStrongPassword();
            if (existedUsername != null)
                throw new BadRequestException(ErrorCodeHelper.Agent.UsernameIsExist);
            if (!isEnoughComlex)
                throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
        }

        private async Task ValidateAgentCredit(IAgentRepository agentRepos, CreateAgentModel model, Data.Entities.Agent clientAgent)
        {
            if (clientAgent.RoleId == (int)Role.Company) return;
            var totalValidCredit = clientAgent.Credit;
            var totalCreditUsed = await agentRepos.SumAllCreditByTypeIdAsync(clientAgent.AgentId, (Role)clientAgent.RoleId);
            if ((totalCreditUsed + model.Credit) > totalValidCredit)
                throw new BadRequestException(ErrorCodeHelper.Agent.InvalidCredit);
        }

        public async Task<GetAgentsResult> GetAgents(GetAgentsModel model)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                       : ClientContext.Agent.ParentId != 0 ? ClientContext.Agent.ParentId
                                                                                           : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId == Role.Agent.ToInt())
            {
                return await GetPlayersByAgent(playerRepos, clientAgent, model);
            }

            return await GetAgentsByRole(agentRepos, clientAgent, model);
        }

        private async Task<GetAgentsResult> GetAgentsByRole(IAgentRepository agentRepos, Data.Entities.Agent clientAgent, GetAgentsModel model)
        {
            IQueryable<Data.Entities.Agent> agentQuery = agentRepos.FindQuery().Include(f => f.AgentSession);

            switch (clientAgent.RoleId.ToEnum<Role>())
            {
                case Role.Company:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Supermaster.ToInt() && x.ParentId == 0L);
                    break;
                case Role.Supermaster:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Master.ToInt() && x.SupermasterId == clientAgent.AgentId && x.ParentId == 0L);
                    break;
                case Role.Master:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Agent.ToInt() && x.SupermasterId == clientAgent.SupermasterId && x.MasterId == clientAgent.AgentId && x.ParentId == 0L);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                agentQuery = agentQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                agentQuery = agentQuery.Where(x => x.State == model.State.Value);
            }

            if (model.SortType == SortType.Descending)
            {
                agentQuery = model.SortName == "state" ? agentQuery.OrderByDescending(x => x.State).ThenBy(x => x.Username) : agentQuery.OrderByDescending(GetSortAgentProperty(model));
            }
            else
            {
                agentQuery = model.SortName == "state" ? agentQuery.OrderBy(x => x.State).ThenBy(x => x.Username) : agentQuery.OrderBy(GetSortAgentProperty(model));
            }
            var result = await agentRepos.PagingByAsync(agentQuery, model.PageIndex, model.PageSize);
            return new GetAgentsResult
            {
                Agents = result.Items.Select(x => new AgentDto
                {
                    Id = x.AgentId,
                    Credit = x.Credit,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MemberMaxCredit = x.MemberMaxCredit,
                    ParentId = x.ParentId,
                    RoleId = x.RoleId,
                    State = x.ParentState.HasValue && x.State >= x.ParentState.Value ? x.State : x.ParentState ?? x.State,
                    Username = x.Username,
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.AgentSession?.IpAddress,
                    Platform = x.AgentSession?.Platform,
                }).ToList(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        private async Task<GetAgentsResult> GetPlayersByAgent(IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, GetAgentsModel model)
        {
            IQueryable<Data.Entities.Player> playerQuery = playerRepos.FindQuery().Include(f => f.PlayerSession);

            playerQuery = playerQuery.Where(x => x.AgentId == clientAgent.AgentId);

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                playerQuery = playerQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                playerQuery = playerQuery.Where(x => x.State == model.State.Value);
            }

            if (model.SortType == SortType.Descending)
            {
                playerQuery = model.SortName == "state" ? playerQuery.OrderByDescending(x => x.State).ThenBy(x => x.Username) : playerQuery.OrderByDescending(GetSortPlayerProperty(model));
            }
            else
            {
                playerQuery = model.SortName == "state" ? playerQuery.OrderBy(x => x.State).ThenBy(x => x.Username) : playerQuery.OrderBy(GetSortPlayerProperty(model));
            }
            var result = await playerRepos.PagingByAsync(playerQuery, model.PageIndex, model.PageSize);
            return new GetAgentsResult
            {
                Agents = result.Items.Select(x => new AgentDto
                {
                    Id = x.PlayerId,
                    Credit = x.Credit,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MemberMaxCredit = null,
                    ParentId = null,
                    RoleId = Role.Player.ToInt(),
                    State = x.ParentState.HasValue && x.State >= x.ParentState.Value ? x.State : x.ParentState ?? x.State,
                    Username = x.Username,
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.PlayerSession != null ? x.PlayerSession.IpAddress : null,
                    Platform = x.PlayerSession != null ? x.PlayerSession.Platform : null
                }).ToList(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        private Expression<Func<Data.Entities.Player, object>> GetSortPlayerProperty(GetAgentsModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return player => player.State;
            return model.SortName?.ToLower() switch
            {
                "username" => player => player.Username,
                "createddate" => player => player.CreatedAt,
                "state" => player => player.State,
                "fullname" => player => (player.FirstName ?? "") + " " + (player.LastName ?? "")
            };
        }

        private static Expression<Func<Data.Entities.Agent, object>> GetSortAgentProperty(GetAgentsModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return agent => agent.State;
            return model.SortName?.ToLower() switch
            {
                "username" => agent => agent.Username,
                "createddate" => agent => agent.CreatedAt,
                "state" => agent => agent.State,
                "fullname" => agent => (agent.FirstName ?? "") + " " + (agent.LastName ?? "")
            };
        }

        public async Task UpdateAgent(UpdateAgentModel updateModel)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var updatedAgent = await agentRepos.FindByIdAsync(updateModel.AgentId) ?? throw new NotFoundException();

            updatedAgent.FirstName = updateModel.FirstName ?? updatedAgent.FirstName;
            updatedAgent.LastName = updateModel.LastName ?? updatedAgent.LastName;
            //  TODO Update children to state.
            var oldStateValue = updatedAgent.State;
            if (updateModel.State.HasValue && updateModel.State.Value != UserState.All.ToInt())
            {
                updatedAgent.State = updateModel.State.Value;
                //Update all children's parent state
                await UpdateAllChildrenParentState(agentRepos, playerRepos, updatedAgent);
            }
            updatedAgent.Permissions = updateModel.Permissions ?? updatedAgent.Permissions;
            updatedAgent.UpdatedAt = ClockService.GetUtcNow();
            updatedAgent.UpdatedBy = ClientContext.Agent.AgentId;
            var oldCreditValue = updatedAgent.Credit;
            updatedAgent.Credit = updateModel.Credit ?? updatedAgent.Credit;
            var oldMemberMaxCreditValue = updatedAgent.MemberMaxCredit ?? 0m;
            updatedAgent.MemberMaxCredit = updateModel.MemberMaxCredit ?? updatedAgent.MemberMaxCredit;
            agentRepos.Update(updatedAgent);

            await LotteryUow.SaveChangesAsync();

            if (updateModel.Credit.HasValue)
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Credit,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = updatedAgent.Username,
                    Action = AuditDataHelper.Credit.Action.ActionUpdateGivenCredit,
                    DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailUpdateGivenCredit, updatedAgent.Username),
                    OldValue = oldCreditValue,
                    NewValue = updatedAgent.Credit,
                    SupermasterId = GetAuditSupermasterId(updatedAgent),
                    MasterId = GetAuditMasterId(updatedAgent)
                });
            }

            if (updateModel.MemberMaxCredit.HasValue)
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.Credit,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = updatedAgent.Username,
                    AgentFirstName = updatedAgent.FirstName,
                    AgentLastName = updatedAgent.LastName,
                    Action = AuditDataHelper.Credit.Action.ActionUpdateGivenCredit,
                    DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailUpdateGivenCreditWithMemberMaxCredit, updatedAgent.Username, updatedAgent.MemberMaxCredit?.ToString("N3", _culture), oldMemberMaxCreditValue.ToString("N3", _culture)),
                    OldValue = oldCreditValue,
                    NewValue = updatedAgent.Credit,
                    SupermasterId = GetAuditSupermasterId(updatedAgent),
                    MasterId = GetAuditMasterId(updatedAgent)
                });
            }

            if (updateModel.State.HasValue && updateModel.State.Value != UserState.All.ToInt())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = (int)AuditType.State,
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = updatedAgent.Username,
                    Action = AuditDataHelper.State.Action.ActionUpdateState,
                    DetailMessage = string.Format(AuditDataHelper.State.DetailMessage.DetailUpdateState, updatedAgent.Username, Enum.GetName(typeof(UserState), oldStateValue), Enum.GetName(typeof(UserState), updatedAgent.State)),
                    OldValue = oldStateValue,
                    NewValue = updatedAgent.State,
                    SupermasterId = GetAuditSupermasterId(updatedAgent),
                    MasterId = GetAuditMasterId(updatedAgent)
                });
            }
        }

        private async Task UpdateAllChildrenParentState(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent parentAgent)
        {
            switch (parentAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    var supermasterAgentChilds = await agentRepos.FindQueryBy(x => x.SupermasterId == parentAgent.AgentId).ToListAsync();
                    var supermasterPlayerChilds = await playerRepos.FindQueryBy(x => x.SupermasterId == parentAgent.AgentId).ToListAsync();
                    supermasterAgentChilds.ForEach(agent =>
                    {
                        agent.ParentState = parentAgent.State;
                    });
                    supermasterPlayerChilds.ForEach(player =>
                    {
                        player.ParentState = parentAgent.State;
                    });
                    break;
                case (int)Role.Master:
                    var masterAgentChilds = await agentRepos.FindQueryBy(x => x.MasterId == parentAgent.AgentId).ToListAsync();
                    var masterPlayerChilds = await playerRepos.FindQueryBy(x => x.MasterId == parentAgent.AgentId).ToListAsync();
                    masterAgentChilds.ForEach(agent =>
                    {
                        agent.ParentState = parentAgent.State;
                    });
                    masterPlayerChilds.ForEach(player =>
                    {
                        player.ParentState = parentAgent.State;
                    });
                    break;
                case (int)Role.Agent:
                    var playerChilds = await playerRepos.FindQueryBy(x => x.AgentId == parentAgent.AgentId).ToListAsync();
                    playerChilds.ForEach(player =>
                    {
                        player.ParentState = parentAgent.State;
                    });
                    break;
            }
        }

        public async Task<bool> CheckExistAgent(string username, bool isSubAgent)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var loginAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (loginAgent.RoleId == Role.Agent.ToInt() && !isSubAgent)
            {
                return await playerRepos.CheckExistPlayer(username);
            }
            return await agentRepos.CheckExistAgent(username);
        }

        public async Task CreateSubAgent(CreateSubAgentModel model)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            var passwordDecoded = model.Password.DecodePassword();

            await ValidateAgentUsernamePassword(agentRepos, model.Username, passwordDecoded);

            await agentRepos.AddAsync(new Data.Entities.Agent
            {
                Username = model.Username,
                Password = passwordDecoded.Md5(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Credit = 0,
                MemberMaxCredit = null,
                RoleId = clientAgent.RoleId,
                Permissions = model.Permissions,
                State = UserState.Open.ToInt(),
                SupermasterId = clientAgent.SupermasterId,
                MasterId = clientAgent.MasterId,
                ParentId = clientAgent.AgentId,
                CreatedAt = ClockService.GetUtcNow()
            });

            await LotteryUow.SaveChangesAsync();
        }

        public async Task<string> GetSuggestionAgentIdentifier(bool isSubAgent)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId == Role.Company.ToInt() && !isSubAgent) return string.Empty;
            var allSubAgents = isSubAgent ? await agentRepos.FindByAsync(x => x.ParentId == clientAgent.AgentId && x.RoleId == clientAgent.RoleId)
                                          : await GetAllAdjacentChildAgent(agentRepos, clientAgent);
            var usernameIdentifies = allSubAgents.Select(x => x.Username.Substring(x.Username.Length - 2, 2)).ToList();
            return isSubAgent ? usernameIdentifies.GetNextDoubleNumerics() : usernameIdentifies.GetNextDoubleCharacters();
        }

        private async Task<IEnumerable<Data.Entities.Agent>> GetAllAdjacentChildAgent(IAgentRepository agentRepos, Data.Entities.Agent clientAgent)
        {
            switch (clientAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    return await agentRepos.FindByAsync(x => x.ParentId == 0 && x.RoleId == clientAgent.RoleId + 1 && x.SupermasterId == clientAgent.AgentId);
                case (int)Role.Master:
                    return await agentRepos.FindByAsync(x => x.ParentId == 0 && x.RoleId == clientAgent.RoleId + 1 && x.MasterId == clientAgent.AgentId);
                default:
                    return new List<Data.Entities.Agent>();
            }
        }

        public async Task<GetAgentDashBoardResult> GetAgentDashBoard()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.ParentId != 0)
            {
                clientAgent = await agentRepos.FindByIdAsync(clientAgent.ParentId) ?? throw new NotFoundException();
            }
            return new GetAgentDashBoardResult
            {
                AgentDashBoard = new AgentDashBoardDto
                {
                    Balance = new BalanceInfo
                    {
                        Username = clientAgent.Username,
                        UserRole = ((Role)clientAgent.RoleId).ToString(),
                        Currency = 0m, // TODO: Handle this later
                        Point = 0, // TODO: Handle this later
                        YesterdayPoint = 0, // TODO: Handle this later
                        Cash = 0m, // TODO: Handle this later
                        YesterdayCash = 0m, // TODO: Handle this later
                        TodayWinLoss = 0m, // TODO: Handle this later
                        Today = ClockService.GetUtcNow(),
                        YesterdayWinLoss = 0m, // TODO: Handle this later
                        Yesterday = ClockService.GetUtcNow().AddDays(-1),
                        TotalGivenOfLowerGrade = (Role)clientAgent.RoleId == Role.Agent
                                                ? await playerRepos.FindQueryBy(x => x.AgentId == clientAgent.AgentId).SumAsync(x => x.Credit)
                                                : await agentRepos.SumAllCreditByTypeIdAsync(clientAgent.AgentId, (Role)clientAgent.RoleId),
                        AgentGiven = (Role)clientAgent.RoleId == Role.Company ? null : clientAgent.Credit
                    },
                    Statistics = new StatisticsInfo
                    {
                        TotalOutstanding = 0m, // TODO: Handle this later
                        AgenStateInfos = await GetAgentStateInfo(agentRepos, playerRepos, clientAgent),
                        TotalNewPlayerOfAMonth = await GetTotalNewPlayerOfAMonth(agentRepos, playerRepos, clientAgent)
                    }
                }
            };
        }

        private async Task<int> GetTotalNewPlayerOfAMonth(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent)
        {
            return clientAgent.RoleId switch
            {
                (int)Role.Company => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month)
                                                            .OrderByDescending(x => x.CreatedAt)
                                                            .CountAsync(),
                (int)Role.Supermaster => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.SupermasterId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .CountAsync(),
                (int)Role.Master => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.MasterId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .CountAsync(),
                (int)Role.Agent => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.AgentId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .CountAsync(),
                _ => 0,
            };
        }

        public async Task<List<AgentSumarryInfo>> GetNewPlayersOfTheMonth()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();
            if (clientAgent.ParentId != 0)
            {
                clientAgent = await agentRepos.FindByIdAsync(clientAgent.ParentId) ?? throw new NotFoundException();
            }

            return clientAgent.RoleId switch
            {
                (int)Role.Company => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month)
                                                            .OrderByDescending(x => x.CreatedAt)
                                                            .Take(AgentHelper.MinimumPlayerQuantity)
                                                            .Select(x => new AgentSumarryInfo
                                                            {
                                                                Username = x.Username,
                                                                FirstName = x.FirstName,
                                                                LastName = x.LastName,
                                                                Credit = x.Credit,
                                                                CreatedDate = x.CreatedAt
                                                            }).ToListAsync(),
                (int)Role.Supermaster => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.SupermasterId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .Take(AgentHelper.MinimumPlayerQuantity)
                                            .Select(x => new AgentSumarryInfo
                                            {
                                                Username = x.Username,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                Credit = x.Credit,
                                                CreatedDate = x.CreatedAt
                                            }).ToListAsync(),
                (int)Role.Master => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.MasterId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .Take(AgentHelper.MinimumPlayerQuantity)
                                            .Select(x => new AgentSumarryInfo
                                            {
                                                Username = x.Username,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                Credit = x.Credit,
                                                CreatedDate = x.CreatedAt
                                            }).ToListAsync(),
                (int)Role.Agent => await playerRepos.FindQueryBy(x => x.CreatedAt.Month == ClockService.GetUtcNow().Month && x.AgentId == clientAgent.AgentId)
                                            .OrderByDescending(x => x.CreatedAt)
                                            .Take(AgentHelper.MinimumPlayerQuantity)
                                            .Select(x => new AgentSumarryInfo
                                            {
                                                Username = x.Username,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                Credit = x.Credit,
                                                CreatedDate = x.CreatedAt
                                            }).ToListAsync(),
                _ => new List<AgentSumarryInfo>(),
            };
        }

        public async Task<List<AgentSumarryInfo>> GetHighestTurnOverPlayersOfTheMonth()
        {
            //TODO: Implement later
            return new List<AgentSumarryInfo>();
        }

        public async Task<List<AgentSumarryInfo>> GetTopPlayersOfTheMonth()
        {
            //TODO: Implement later
            return new List<AgentSumarryInfo>();
        }

        private async Task<List<AgenStateInfo>> GetAgentStateInfo(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent)
        {
            var result = new List<AgenStateInfo>();
            switch (clientAgent.RoleId)
            {
                case (int)Role.Company:
                    await GetAgentStateForUserCompany(agentRepos, playerRepos, clientAgent, result);
                    break;
                case (int)Role.Supermaster:
                    await GetAgentStateForUserSuperMaster(agentRepos, playerRepos, clientAgent, result);
                    break;
                case (int)Role.Master:
                    await GetAgentStateForUserMaster(agentRepos, playerRepos, clientAgent, result);
                    break;
                case (int)Role.Agent:
                    await GetAgentStateForUserAgent(agentRepos, playerRepos, clientAgent, result);
                    break;
            }

            return result;
        }

        private async Task GetAgentStateForUserAgent(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, List<AgenStateInfo> result)
        {
            //Get Player State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Player.ToString(), new List<long> { clientAgent.AgentId }, Role.Agent));
        }

        private async Task GetAgentStateForUserMaster(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, List<AgenStateInfo> result)
        {
            //Get Agent State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Agent.ToString(), new List<long> { clientAgent.AgentId }, Role.Master));
            var listAgentIds = await agentRepos.FindQueryBy(x => x.MasterId == clientAgent.AgentId && x.RoleId == (int)Role.Agent)
                                               .Select(x => x.AgentId)
                                               .ToListAsync();

            //Get Player State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Player.ToString(), listAgentIds, Role.Agent));
        }

        private async Task GetAgentStateForUserSuperMaster(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, List<AgenStateInfo> result)
        {
            //Add Master State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Master.ToString(), new List<long> { clientAgent.AgentId }, Role.Supermaster));
            var listMasterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == clientAgent.AgentId && x.RoleId == (int)Role.Master)
                                                .Select(x => x.AgentId)
                                                .ToListAsync();

            //Get Agent State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Agent.ToString(), listMasterIds, Role.Master));
            var listAgentIds = await agentRepos.FindQueryBy(x => listMasterIds.Contains(x.MasterId) && x.RoleId == (int)Role.Agent)
                                               .Select(x => x.AgentId)
                                               .ToListAsync();

            //Get Player State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Player.ToString(), listAgentIds, Role.Agent));
        }

        private async Task GetAgentStateForUserCompany(IAgentRepository agentRepos, IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, List<AgenStateInfo> result)
        {
            //Add Supermaster State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Supermaster.ToString(), new List<long> { clientAgent.AgentId }, Role.Company));
            var listSuperMasterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == clientAgent.AgentId && x.MasterId == clientAgent.AgentId && x.RoleId == (int)Role.Supermaster)
                                                     .Select(x => x.AgentId)
                                                     .ToListAsync();

            //Add Master State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Master.ToString(), listSuperMasterIds, Role.Supermaster));
            var listMasterIds = await agentRepos.FindQueryBy(x => listSuperMasterIds.Contains(x.SupermasterId) && x.RoleId == (int)Role.Master)
                                                .Select(x => x.AgentId)
                                                .ToListAsync();

            //Get Agent State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Agent.ToString(), listMasterIds, Role.Master));
            var listAgentIds = await agentRepos.FindQueryBy(x => listMasterIds.Contains(x.MasterId) && x.RoleId == (int)Role.Agent)
                                               .Select(x => x.AgentId)
                                               .ToListAsync();

            //Get Player State Info
            result.Add(await GetTargetAgentStateInfo(agentRepos, playerRepos, Role.Player.ToString(), listAgentIds, Role.Agent));
        }

        private async Task<AgenStateInfo> GetTargetAgentStateInfo(IAgentRepository agentRepos, IPlayerRepository playerRepos, string agentName, List<long> parentAgentIds, Role parentRole)
        {
            if (parentRole == Role.Agent)
            {
                return new AgenStateInfo
                {
                    AgentName = agentName,
                    TotalAgentOpen = await playerRepos.FindQueryBy(x => parentAgentIds.Contains(x.AgentId) && x.State == (int)UserState.Open).CountAsync(),
                    TotalAgentClosed = await playerRepos.FindQueryBy(x => parentAgentIds.Contains(x.AgentId) && x.State == (int)UserState.Closed).CountAsync(),
                    TotalAgentSuspended = await playerRepos.FindQueryBy(x => parentAgentIds.Contains(x.AgentId) && x.State == (int)UserState.Suspended).CountAsync()
                };
            }
            return new AgenStateInfo
            {
                AgentName = agentName,
                TotalAgentOpen = await agentRepos.CountAgentByState(parentAgentIds, parentRole, UserState.Open),
                TotalAgentClosed = await agentRepos.CountAgentByState(parentAgentIds, parentRole, UserState.Closed),
                TotalAgentSuspended = await agentRepos.CountAgentByState(parentAgentIds, parentRole, UserState.Suspended)
            };
        }

        public async Task<GetAgentCreditInfoResult> GetAgentCreditInfo(long? agentId)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var targetAgentId = agentId.HasValue ? agentId.Value
                                                : ClientContext.Agent.ParentId == 0
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var targetAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();

            var totalCreditUsed = targetAgent.RoleId == Role.Agent.ToInt() ? await playerRepos.FindQueryBy(x => x.AgentId == targetAgent.AgentId).SumAsync(x => x.Credit)
                                                                           : await agentRepos.SumAllCreditByTypeIdAsync(targetAgent.AgentId, (Role)targetAgent.RoleId);

            return new GetAgentCreditInfoResult
            {
                AvailableGivenCredit = targetAgent.RoleId == Role.Agent.ToInt() && targetAgent.MemberMaxCredit.HasValue ? Math.Min(targetAgent.MemberMaxCredit.Value, targetAgent.Credit - totalCreditUsed)
                                                                                                                        : targetAgent.Credit - totalCreditUsed
            };
        }

        public async Task<List<AgentBreadCrumbsDto>> GetBreadCrumbs(long? agentId, int? roleId)
        {
            if (!agentId.HasValue || !roleId.HasValue) return new List<AgentBreadCrumbsDto>();
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var loginAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();

            var allPreviousNodes = new List<AgentBreadCrumbsDto>();
            if (roleId.Value != Role.Player.ToInt())
            {
                var targetAgent = await agentRepos.FindByIdAsync(agentId.Value) ?? throw new NotFoundException();
                switch (targetAgent.RoleId)
                {
                    case (int)Role.Company:
                        return new List<AgentBreadCrumbsDto>();
                    case (int)Role.Supermaster:
                        allPreviousNodes = await GetSuperMasterBreadCrumbs(agentRepos, targetAgent);
                        break;
                    case (int)Role.Master:
                        allPreviousNodes = await GetMasterBreadCrumbs(agentRepos, targetAgent);
                        break;
                    case (int)Role.Agent:
                        allPreviousNodes = await GetAgentBreadCrumbs(agentRepos, targetAgent);
                        break;
                    default:
                        return new List<AgentBreadCrumbsDto>();
                }
            }
            else
            {
                var targetPlayer = await playerRepos.FindByIdAsync(agentId.Value) ?? throw new NotFoundException();
                allPreviousNodes = await GetPlayerBreadCrumbs(agentRepos, targetPlayer);
            }

            return allPreviousNodes.Where(x => x.RoleId >= loginAgent.RoleId).ToList();
        }

        private async Task<List<AgentBreadCrumbsDto>> GetPlayerBreadCrumbs(IAgentRepository agentRepos, Data.Entities.Player targetPlayer)
        {
            var agent = await agentRepos.FindQueryBy(x => x.RoleId == Role.Agent.ToInt() && x.AgentId == targetPlayer.AgentId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var master = await agentRepos.FindQueryBy(x => x.RoleId == Role.Master.ToInt() && x.AgentId == targetPlayer.MasterId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var supermaster = await agentRepos.FindQueryBy(x => x.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetPlayer.SupermasterId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var company = await agentRepos.FindQueryBy(x => x.RoleId == Role.Company.ToInt() && x.ParentId == 0L).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return new List<AgentBreadCrumbsDto>
                    {
                        new AgentBreadCrumbsDto{ AgentId = company.AgentId, RoleId = Role.Company.ToInt(), Username = company.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = supermaster.AgentId, RoleId = Role.Supermaster.ToInt(), Username = supermaster.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = master.AgentId, RoleId = Role.Master.ToInt(), Username = master.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = agent.AgentId, RoleId = Role.Agent.ToInt(), Username = agent.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = targetPlayer.PlayerId, RoleId = Role.Player.ToInt(), Username = targetPlayer.Username, IsActive = true}
                    };
        }

        private async Task<List<AgentBreadCrumbsDto>> GetAgentBreadCrumbs(IAgentRepository agentRepos, Data.Entities.Agent targetAgent)
        {
            var master = await agentRepos.FindQueryBy(x => x.RoleId == Role.Master.ToInt() && x.AgentId == targetAgent.MasterId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var supermaster = await agentRepos.FindQueryBy(x => x.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var company = await agentRepos.FindQueryBy(x => x.RoleId == Role.Company.ToInt() && x.ParentId == 0L).FirstOrDefaultAsync() ?? throw new NotFoundException();

            return new List<AgentBreadCrumbsDto>
                    {
                        new AgentBreadCrumbsDto{ AgentId = company.AgentId, RoleId = Role.Company.ToInt(), Username = company.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = supermaster.AgentId, RoleId = Role.Supermaster.ToInt(), Username = supermaster.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = master.AgentId, RoleId = Role.Master.ToInt(), Username = master.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = targetAgent.AgentId, RoleId = Role.Agent.ToInt(), Username = targetAgent.Username, IsActive = true}
                    };
        }

        private async Task<List<AgentBreadCrumbsDto>> GetMasterBreadCrumbs(IAgentRepository agentRepos, Data.Entities.Agent targetAgent)
        {
            var supermaster = await agentRepos.FindQueryBy(x => x.RoleId == Role.Supermaster.ToInt() && x.AgentId == targetAgent.SupermasterId).FirstOrDefaultAsync() ?? throw new NotFoundException();
            var company = await agentRepos.FindQueryBy(x => x.RoleId == Role.Company.ToInt() && x.ParentId == 0L).FirstOrDefaultAsync() ?? throw new NotFoundException();

            return new List<AgentBreadCrumbsDto>
                    {
                        new AgentBreadCrumbsDto{ AgentId = company.AgentId, RoleId = Role.Company.ToInt(), Username = company.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = supermaster.AgentId, RoleId = Role.Supermaster.ToInt(), Username = supermaster.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = targetAgent.AgentId, RoleId = Role.Master.ToInt(), Username = targetAgent.Username, IsActive = true}
                    };
        }

        private async Task<List<AgentBreadCrumbsDto>> GetSuperMasterBreadCrumbs(IAgentRepository agentRepos, Data.Entities.Agent targetAgent)
        {
            var company = await agentRepos.FindQueryBy(x => x.RoleId == Role.Company.ToInt() && x.ParentId == 0L).FirstOrDefaultAsync() ?? throw new NotFoundException();
            return new List<AgentBreadCrumbsDto>
                    {
                        new AgentBreadCrumbsDto{ AgentId = company.AgentId, RoleId = Role.Company.ToInt(), Username = company.Username, IsActive = false },
                        new AgentBreadCrumbsDto{ AgentId = targetAgent.AgentId, RoleId = Role.Supermaster.ToInt(), Username = targetAgent.Username, IsActive = true}
                    };
        }

        public async Task<GetSubAgentsResult> GetSubAgents()
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();

            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();

            return new GetSubAgentsResult
            {
                SubAgents = agentRepos.FindQueryBy(x => x.ParentId == clientAgent.AgentId).Include(f => f.AgentSession)
                                      .OrderBy(x => x.State)
                                      .ThenBy(x => x.Username)
                                      .Select(x => new SubAgentDto
                                      {
                                          Id = x.AgentId,
                                          Username = x.Username,
                                          State = x.State,
                                          FirstName = x.FirstName,
                                          LastName = x.LastName,
                                          Role = x.RoleId,
                                          CreatedDate = x.CreatedAt,
                                          Permissions = !string.IsNullOrEmpty(x.Permissions) ? x.Permissions.Split(',', StringSplitOptions.None).ToList() : new List<string>(),
                                          IpAddress = x.AgentSession != null ? x.AgentSession.IpAddress : null,
                                          Platform = x.AgentSession != null ? x.AgentSession.Platform : null
                                      })
            };
        }

        public async Task<GetAgentWinLossSummaryResult> GetAgentWinLossSummary(long? agentId, DateTime from, DateTime to, bool selectedDraft)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var ticketRepos = LotteryUow.GetRepository<ITicketRepository>();
            var targetAgentId = agentId.HasValue ? agentId.Value
                                                : ClientContext.Agent.ParentId == 0
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var loginAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var ticketStates = selectedDraft ? CommonHelper.AllTicketState() : CommonHelper.CompletedTicketWithoutRefundOrRejectState();
            switch (loginAgent.RoleId)
            {
                case (int)Role.Company:
                    return await GetAgentWinLossOfCompany(agentRepos, ticketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Supermaster:
                    return await GetAgentWinLossOfSupermaster(agentRepos, ticketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Master:
                    return await GetAgentWinLossOfMaster(agentRepos, ticketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Agent:
                    return await GetAgentWinLossOfAgent(playerRepos, ticketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                default:
                    return new GetAgentWinLossSummaryResult();
            };
        }

        private async Task<GetAgentWinLossSummaryResult> GetAgentWinLossOfAgent(IPlayerRepository playerRepos, ITicketRepository ticketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var winLossInfos = new List<WinLoseInfo>
            {
                new() {
                    WinLose = 0m,
                    Commission = 0m
                }
            };
            var agentWinlossSummaries = await playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId)).Include(x => x.PlayerSession)
                                   .Join(ticketRepos.FindQueryBy(y => !y.ParentId.HasValue && ticketStates.Contains(y.State) && y.KickOffTime >= from.Date && y.KickOffTime <= to.AddDays(1).AddTicks(-1)),
                                   x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       player.PlayerSession.IpAddress,
                                       player.PlayerSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.PlayerWinLoss,
                                       ticket.DraftPlayerWinLoss,
                                       ticket.AgentWinLoss,
                                       ticket.AgentCommission,
                                       ticket.DraftAgentWinLoss,
                                       ticket.DraftAgentCommission,
                                       ticket.MasterWinLoss,
                                       ticket.MasterCommission,
                                       ticket.DraftMasterWinLoss,
                                       ticket.DraftMasterCommission,
                                       ticket.SupermasterWinLoss,
                                       ticket.SupermasterCommission,
                                       ticket.DraftSupermasterWinLoss,
                                       ticket.DraftSupermasterCommission,
                                       ticket.CompanyWinLoss,
                                       ticket.DraftCompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       RoleId = Role.Player.ToInt(),
                                       BetCount = x.Count(),
                                       Point = x.Sum(s => s.Stake),
                                       Payout = x.Sum(s => s.PlayerPayout),
                                       WinLose = x.Sum(s => s.PlayerWinLoss),
                                       DraftWinLose = x.Sum(s => s.DraftPlayerWinLoss),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = x.Sum(s => s.AgentCommission),
                                                           Subtotal = x.Sum(s => s.AgentWinLoss) + x.Sum(s => s.AgentCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftAgentWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftAgentCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftAgentWinLoss) + x.Sum(s => s.DraftAgentCommission)
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = x.Sum(s => s.MasterCommission),
                                                           Subtotal = x.Sum(s => s.MasterWinLoss) + x.Sum(s => s.MasterCommission),

                                                           DraftWinLose = x.Sum(s => s.DraftMasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftMasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftMasterWinLoss) + x.Sum(s => s.DraftMasterCommission)
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = x.Sum(s => s.SupermasterCommission),
                                                           Subtotal = x.Sum(s => s.SupermasterWinLoss) + x.Sum(s => s.SupermasterCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftSupermasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftSupermasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftSupermasterWinLoss) + x.Sum(s => s.DraftSupermasterCommission)
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.MasterWinLoss),
                                       DraftCompany = x.Sum(s => s.DraftMasterWinLoss),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetAgentWinLossSummaryResult
            {
                AgentWinlossSummaries = agentWinlossSummaries,
                TotalBetCount = agentWinlossSummaries.Sum(x => x.BetCount),
                TotalPoint = agentWinlossSummaries.Sum(x => x.Point),
                TotalPayout = agentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = agentWinlossSummaries.Sum(x => x.WinLose),
                TotalDraftWinLose = agentWinlossSummaries.Sum(x => x.DraftWinLose),
                TotalAgentWinLoseInfo = new List<TotalAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftSubtotal),
                        RoleId = Role.Agent.ToInt()
                    }
                },
                TotalCompany = agentWinlossSummaries.Sum(x => x.Company),
                TotalDraftCompany = agentWinlossSummaries.Sum(x => x.DraftCompany)
            };
        }

        private async Task<GetAgentWinLossSummaryResult> GetAgentWinLossOfMaster(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var winLossInfos = new List<WinLoseInfo>
            {
                new() {
                    WinLose = 0m,
                    Commission = 0m
                },
                new() {
                    WinLose = 0m,
                    Commission = 0m
                }
            };
            var agentWinlossSummaries = await agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQueryBy(y => !y.ParentId.HasValue && ticketStates.Contains(y.State) && y.KickOffTime >= from.Date && y.KickOffTime <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.PlayerWinLoss,
                                       ticket.DraftPlayerWinLoss,
                                       ticket.AgentWinLoss,
                                       ticket.AgentCommission,
                                       ticket.DraftAgentWinLoss,
                                       ticket.DraftAgentCommission,
                                       ticket.MasterWinLoss,
                                       ticket.MasterCommission,
                                       ticket.DraftMasterWinLoss,
                                       ticket.DraftMasterCommission,
                                       ticket.SupermasterWinLoss,
                                       ticket.SupermasterCommission,
                                       ticket.DraftSupermasterWinLoss,
                                       ticket.DraftSupermasterCommission,
                                       ticket.CompanyWinLoss,
                                       ticket.DraftCompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.Count(),
                                       Point = x.Sum(s => s.Stake),
                                       Payout = x.Sum(s => s.PlayerPayout),
                                       WinLose = x.Sum(s => s.PlayerWinLoss),
                                       DraftWinLose = x.Sum(s => s.DraftPlayerWinLoss),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = x.Sum(s => s.AgentCommission),
                                                           Subtotal = x.Sum(s => s.AgentWinLoss) + x.Sum(s => s.AgentCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftAgentWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftAgentCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftAgentWinLoss) + x.Sum(s => s.DraftAgentCommission)
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = x.Sum(s => s.MasterCommission),
                                                           Subtotal = x.Sum(s => s.MasterWinLoss) + x.Sum(s => s.MasterCommission),

                                                           DraftWinLose = x.Sum(s => s.DraftMasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftMasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftMasterWinLoss) + x.Sum(s => s.DraftMasterCommission)
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = x.Sum(s => s.SupermasterCommission),
                                                           Subtotal = x.Sum(s => s.SupermasterWinLoss) + x.Sum(s => s.SupermasterCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftSupermasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftSupermasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftSupermasterWinLoss) + x.Sum(s => s.DraftSupermasterCommission)
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.SupermasterWinLoss),
                                       DraftCompany = x.Sum(s => s.DraftSupermasterWinLoss),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetAgentWinLossSummaryResult
            {
                AgentWinlossSummaries = agentWinlossSummaries,
                TotalBetCount = agentWinlossSummaries.Sum(x => x.BetCount),
                TotalPoint = agentWinlossSummaries.Sum(x => x.Point),
                TotalPayout = agentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = agentWinlossSummaries.Sum(x => x.WinLose),
                TotalDraftWinLose = agentWinlossSummaries.Sum(x => x.DraftWinLose),
                TotalAgentWinLoseInfo = new List<TotalAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftSubtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftSubtotal),
                        RoleId = Role.Master.ToInt()
                    }
                },
                TotalCompany = agentWinlossSummaries.Sum(x => x.Company),
                TotalDraftCompany = agentWinlossSummaries.Sum(x => x.DraftCompany)
            };
        }

        private async Task<GetAgentWinLossSummaryResult> GetAgentWinLossOfSupermaster(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var winLossInfos = new List<WinLoseInfo>
            {
                new() {
                    WinLose = 0m,
                    Commission = 0m
                },
                new() {
                    WinLose = 0m,
                    Commission = 0m
                },
                new() {
                    WinLose = 0m,
                    Commission = 0m
                }
            };
            var agentWinlossSummaries = await agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQueryBy(y => !y.ParentId.HasValue && ticketStates.Contains(y.State) && y.KickOffTime >= from.Date && y.KickOffTime <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.Stake,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.PlayerPayout,
                                       ticket.PlayerWinLoss,
                                       ticket.DraftPlayerWinLoss,
                                       ticket.AgentWinLoss,
                                       ticket.AgentCommission,
                                       ticket.DraftAgentWinLoss,
                                       ticket.DraftAgentCommission,
                                       ticket.MasterWinLoss,
                                       ticket.MasterCommission,
                                       ticket.DraftMasterWinLoss,
                                       ticket.DraftMasterCommission,
                                       ticket.SupermasterWinLoss,
                                       ticket.SupermasterCommission,
                                       ticket.DraftSupermasterWinLoss,
                                       ticket.DraftSupermasterCommission,
                                       ticket.CompanyWinLoss,
                                       ticket.DraftCompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.Count(),
                                       Point = x.Sum(s => s.Stake),
                                       Payout = x.Sum(s => s.PlayerPayout),
                                       WinLose = x.Sum(s => s.PlayerWinLoss),
                                       DraftWinLose = x.Sum(s => s.DraftPlayerWinLoss),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = x.Sum(s => s.AgentCommission),
                                                           Subtotal = x.Sum(s => s.AgentWinLoss) + x.Sum(s => s.AgentCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftAgentWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftAgentCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftAgentWinLoss) + x.Sum(s => s.DraftAgentCommission)
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = x.Sum(s => s.MasterCommission),
                                                           Subtotal = x.Sum(s => s.MasterWinLoss) + x.Sum(s => s.MasterCommission),

                                                           DraftWinLose = x.Sum(s => s.DraftMasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftMasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftMasterWinLoss) + x.Sum(s => s.DraftMasterCommission)
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = x.Sum(s => s.SupermasterCommission),
                                                           Subtotal = x.Sum(s => s.SupermasterWinLoss) + x.Sum(s => s.SupermasterCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftSupermasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftSupermasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftSupermasterWinLoss) + x.Sum(s => s.DraftSupermasterCommission)
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.CompanyWinLoss),
                                       DraftCompany = x.Sum(s => s.DraftCompanyWinLoss),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetAgentWinLossSummaryResult
            {
                AgentWinlossSummaries = agentWinlossSummaries,
                TotalBetCount = agentWinlossSummaries.Sum(x => x.BetCount),
                TotalPoint = agentWinlossSummaries.Sum(x => x.Point),
                TotalPayout = agentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = agentWinlossSummaries.Sum(x => x.WinLose),
                TotalAgentWinLoseInfo = new List<TotalAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftSubtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftSubtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftSubtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalCompany = agentWinlossSummaries.Sum(x => x.Company),
                TotalDraftCompany = agentWinlossSummaries.Sum(x => x.DraftCompany)
            };
        }

        private async Task<GetAgentWinLossSummaryResult> GetAgentWinLossOfCompany(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var agentWinlossSummaries = await agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQueryBy(y => !y.ParentId.HasValue && ticketStates.Contains(y.State) && y.KickOffTime >= from.Date && y.KickOffTime <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.Stake,
                                       //   Player
                                       ticket.PlayerPayout,
                                       ticket.PlayerWinLoss,
                                       ticket.DraftPlayerWinLoss,
                                       //   Agent
                                       ticket.AgentWinLoss,
                                       ticket.AgentCommission,
                                       ticket.DraftAgentWinLoss,
                                       ticket.DraftAgentCommission,
                                       //   Master
                                       ticket.MasterWinLoss,
                                       ticket.MasterCommission,
                                       ticket.DraftMasterWinLoss,
                                       ticket.DraftMasterCommission,
                                       //   Supermaster
                                       ticket.SupermasterWinLoss,
                                       ticket.SupermasterCommission,
                                       ticket.DraftSupermasterWinLoss,
                                       ticket.DraftSupermasterCommission,
                                       //   Company
                                       ticket.CompanyWinLoss,
                                       ticket.DraftCompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.Count(),
                                       Point = x.Sum(s => s.Stake),
                                       Payout = x.Sum(s => s.PlayerPayout),
                                       WinLose = x.Sum(s => s.PlayerWinLoss),
                                       DraftWinLose = x.Sum(s => s.DraftPlayerWinLoss),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = x.Sum(s => s.AgentCommission),
                                                           Subtotal = x.Sum(s => s.AgentWinLoss) + x.Sum(s => s.AgentCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftAgentWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftAgentCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftAgentWinLoss) + x.Sum(s => s.DraftAgentCommission)
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = x.Sum(s => s.MasterCommission),
                                                           Subtotal = x.Sum(s => s.MasterWinLoss) + x.Sum(s => s.MasterCommission),

                                                           DraftWinLose = x.Sum(s => s.DraftMasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftMasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftMasterWinLoss) + x.Sum(s => s.DraftMasterCommission)
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new WinLoseInfo
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = x.Sum(s => s.SupermasterCommission),
                                                           Subtotal = x.Sum(s => s.SupermasterWinLoss) + x.Sum(s => s.SupermasterCommission),
                                                           DraftWinLose = x.Sum(s => s.DraftSupermasterWinLoss),
                                                           DraftCommission = x.Sum(s => s.DraftSupermasterCommission),
                                                           DraftSubtotal = x.Sum(s => s.DraftSupermasterWinLoss) + x.Sum(s => s.DraftSupermasterCommission)
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.CompanyWinLoss),
                                       DraftCompany = x.Sum(s => s.DraftCompanyWinLoss),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetAgentWinLossSummaryResult
            {
                AgentWinlossSummaries = agentWinlossSummaries,
                TotalAgentWinLoseInfo = new List<TotalAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.DraftSubtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.DraftSubtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalDraftWinLose = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftWinLose),
                        TotalCommission = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalDraftCommission = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftCommission),
                        TotalSubTotal = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        TotalDraftSubTotal = agentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.DraftSubtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalBetCount = agentWinlossSummaries.Sum(x => x.BetCount),
                TotalPoint = agentWinlossSummaries.Sum(x => x.Point),
                TotalPayout = agentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = agentWinlossSummaries.Sum(x => x.WinLose),
                TotalDraftWinLose = agentWinlossSummaries.Sum(x => x.DraftWinLose),
                TotalCompany = agentWinlossSummaries.Sum(x => x.Company),
                TotalDraftCompany = agentWinlossSummaries.Sum(x => x.DraftCompany)
            };
        }

        public async Task UpdateAgentBetSetting(long agentId, List<AgentBetSettingDto> updateItems)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();

            var agent = await agentRepository.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var auditBetSettings = new List<AuditSettingData>();
            var dictPlayerBetSettings = new Dictionary<long, Dictionary<int, BetSettingModel>>();
            var updateBetKindIds = updateItems.Select(x => x.BetKindId);
            var updatedBetKinds = await betKindRepos.FindQueryBy(x => updateBetKindIds.Contains(x.Id)).ToListAsync();
            var existedAgentBetSettings = await agentOddRepository.FindQueryBy(x => x.AgentId == agent.AgentId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            var childAgentIds = await GetChildAgentIds(agentRepository, agent);
            var childPlayerIds = await GetChildPlayerIds(playerRepository, agent);
            var existedChildAgentBetSettings = await agentOddRepository.FindQuery().Include(x => x.Agent).Where(x => childAgentIds.Contains(x.AgentId) && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            var existedChildPlayerBetSettings = await playerOddRepository.FindQuery().Include(x => x.Player).Where(x => childPlayerIds.Contains(x.PlayerId) && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            existedAgentBetSettings.ForEach(item =>
            {
                var updatedChildAgentItems = existedChildAgentBetSettings.Where(x => x.BetKindId == item.BetKindId).ToList();
                var updatedChildPlayerItems = existedChildPlayerBetSettings.Where(x => x.BetKindId == item.BetKindId).ToList();
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if (updateItem != null)
                {
                    var oldBuyValue = item.Buy;
                    var oldMinBetValue = item.MinBet;
                    var oldMaxBetValue = item.MaxBet;
                    var oldMaxPerNumber = item.MaxPerNumber;
                    item.Buy = updateItem.ActualBuy;
                    item.MinBet = updateItem.ActualMinBet;
                    item.MaxBet = updateItem.ActualMaxBet;
                    item.MaxPerNumber = updateItem.ActualMaxPerNumber;

                    // Update all children of target agent if new value of agent is lower than the oldest one
                    updatedChildAgentItems.ForEach(childAgentItem =>
                    {
                        // Update maxBet, maxPerNumber
                        childAgentItem.MaxBet = item.MaxBet < childAgentItem.MaxBet ? item.MaxBet : childAgentItem.MaxBet;
                        childAgentItem.MaxPerNumber = item.MaxPerNumber < childAgentItem.MaxPerNumber ? item.MaxPerNumber : childAgentItem.MaxPerNumber;
                        if (childAgentItem.MaxBet > childAgentItem.MaxPerNumber)
                        {
                            childAgentItem.MaxBet = childAgentItem.MaxPerNumber;
                        }

                        // Update minBuy, actualBuy
                        childAgentItem.MinBuy = item.Buy;
                        if (childAgentItem.MinBuy > childAgentItem.Buy)
                        {
                            childAgentItem.Buy = childAgentItem.MinBuy;
                        }
                    });

                    updatedChildPlayerItems.ForEach(childPlayerItem =>
                    {
                        childPlayerItem.Buy = item.Buy < childPlayerItem.Buy ? item.Buy : childPlayerItem.Buy;
                        childPlayerItem.MaxBet = item.MaxBet < childPlayerItem.MaxBet ? item.MaxBet : childPlayerItem.MaxBet;
                        childPlayerItem.MaxPerNumber = item.MaxPerNumber < childPlayerItem.MaxPerNumber ? item.MaxPerNumber : childPlayerItem.MaxPerNumber;
                        if (childPlayerItem.MaxBet > childPlayerItem.MaxPerNumber)
                        {
                            childPlayerItem.MaxBet = childPlayerItem.MaxPerNumber;
                        }

                        if (!dictPlayerBetSettings.TryGetValue(childPlayerItem.PlayerId, out Dictionary<int, BetSettingModel> playerBetSettings))
                        {
                            playerBetSettings = new Dictionary<int, BetSettingModel>();
                            dictPlayerBetSettings[childPlayerItem.PlayerId] = playerBetSettings;
                        }
                        playerBetSettings[childPlayerItem.BetKindId] = new BetSettingModel
                        {
                            MinBet = childPlayerItem.MinBet,
                            MaxBet = childPlayerItem.MaxBet,
                            MaxPerNumber = childPlayerItem.MaxPerNumber,
                            OddsValue = childPlayerItem.Buy
                        };
                    });

                    if (oldMinBetValue == item.MinBet && oldMaxBetValue == item.MaxBet && oldMaxPerNumber == item.MaxPerNumber) return;
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

            if (existedAgentBetSettings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = agent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdateBetSetting,
                    SupermasterId = GetAuditSupermasterId(agent),
                    MasterId = GetAuditMasterId(agent),
                    AuditSettingDatas = auditBetSettings.OrderBy(x => x.BetKind).ToList()
                });
            }

            await InternalProcessPlayerBetSetting(dictPlayerBetSettings);
        }

        private async Task<List<long>> GetChildPlayerIds(IPlayerRepository playerRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Supermaster:
                    return await playerRepository.FindQueryBy(x => x.SupermasterId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                case (int)Role.Master:
                    return await playerRepository.FindQueryBy(x => x.MasterId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                case (int)Role.Agent:
                    return await playerRepository.FindQueryBy(x => x.AgentId == agent.AgentId).Select(x => x.PlayerId).ToListAsync();
                default:
                    return new List<long>();
            }
        }

        private async Task<List<long>> GetChildAgentIds(IAgentRepository agentRepository, Data.Entities.Agent agent)
        {
            switch (agent.RoleId)
            {
                case (int)Role.Supermaster:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.SupermasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                case (int)Role.Master:
                    return await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId && x.MasterId == agent.AgentId && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
                default:
                    return new List<long>();
            }
        }

        private List<AgentOdd> GetAdjacentChildAgentBetSettings(List<AgentOdd> existedChildAgentBetSettings, Data.Entities.Agent clientAgent)
        {
            switch (clientAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    return existedChildAgentBetSettings.Where(x => x.Agent.RoleId == clientAgent.RoleId + 1 && x.Agent.SupermasterId == clientAgent.AgentId).ToList();
                case (int)Role.Master:
                    return existedChildAgentBetSettings.Where(x => x.Agent.RoleId == clientAgent.RoleId + 1 && x.Agent.MasterId == clientAgent.AgentId).ToList();
                default:
                    return new List<AgentOdd>();
            }
        }

        private async Task InternalProcessPlayerBetSetting(Dictionary<long, Dictionary<int, BetSettingModel>> dictPlayerBetSettings)
        {
            foreach (var item in dictPlayerBetSettings)
            {
                await _playerSettingService.BuildSettingByBetKindCache(item.Key, item.Value);
            }
        }

        public async Task UpdateAgentPositionTaking(long agentId, List<AgentPositionTakingDto> updateItems)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var agentPositionTakingRepository = LotteryUow.GetRepository<IAgentPositionTakingRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();

            var agent = await agentRepository.FindByIdAsync(agentId) ?? throw new NotFoundException();

            var auditPositionTakings = new List<AuditSettingData>();
            var updateBetKindIds = updateItems.Select(x => x.BetKindId);
            var updatedBetKinds = await betKindRepos.FindQueryBy(x => updateBetKindIds.Contains(x.Id)).ToListAsync();
            var existedAgentPositionTakings = await agentPositionTakingRepository.FindQueryBy(x => x.AgentId == agent.AgentId && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            var childAgentIds = await agentRepository.FindQueryBy(x => x.RoleId > agent.RoleId).Select(x => x.AgentId).ToListAsync();
            var existedChildAgentPositionTakings = await agentPositionTakingRepository.FindQueryBy(x => childAgentIds.Contains(x.AgentId) && updateBetKindIds.Contains(x.BetKindId)).ToListAsync();
            existedAgentPositionTakings.ForEach(item =>
            {
                var updatedChildAgentItems = existedChildAgentPositionTakings.Where(x => x.BetKindId == item.BetKindId).ToList();
                var updateItem = updateItems.FirstOrDefault(x => x.BetKindId == item.BetKindId);
                if (updateItem != null)
                {
                    var oldPTValue = item.PositionTaking;
                    item.PositionTaking = updateItem.ActualPositionTaking;

                    // Update all children of target agent if new value of agent is lower than the oldest one
                    updatedChildAgentItems.ForEach(childItem =>
                    {
                        childItem.PositionTaking = item.PositionTaking < childItem.PositionTaking ? item.PositionTaking : childItem.PositionTaking;
                    });

                    auditPositionTakings.AddRange(new List<AuditSettingData>
                    {
                        new AuditSettingData
                        {
                            Title = AuditDataHelper.Setting.PositionTakingTitle,
                            BetKind = updatedBetKinds.FirstOrDefault(x => x.Id == updateItem.BetKindId)?.Name,
                            OldValue = oldPTValue,
                            NewValue = item.PositionTaking
                        }
                    });
                }
            });
            await LotteryUow.SaveChangesAsync();

            if (existedAgentPositionTakings.Any())
            {
                await _auditService.SaveAuditData(new AuditParams
                {
                    Type = AuditType.Setting.ToInt(),
                    EditedUsername = ClientContext.Agent.UserName,
                    AgentUserName = agent.Username,
                    Action = AuditDataHelper.Setting.Action.ActionUpdatePositionTaking,
                    SupermasterId = GetAuditSupermasterId(agent),
                    MasterId = GetAuditMasterId(agent),
                    AuditSettingDatas = auditPositionTakings.OrderBy(x => x.BetKind).ToList()
                });
            }
        }

        public async Task<GetAgentCreditBalanceResult> GetAgentCreditBalances(GetAgentCreditBalanceModel model)
        {
            //Init repos
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                       : ClientContext.Agent.ParentId != 0 ? ClientContext.Agent.ParentId
                                                                                           : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            if (clientAgent.RoleId == Role.Agent.ToInt())
            {
                return await GetPlayerCreditBalance(playerRepos, clientAgent, model);
            }

            return await GetAgentCreditBalance(agentRepos, clientAgent, model);
        }

        private async Task<GetAgentCreditBalanceResult> GetPlayerCreditBalance(IPlayerRepository playerRepos, Data.Entities.Agent clientAgent, GetAgentCreditBalanceModel model)
        {
            IQueryable<Data.Entities.Player> playerQuery = playerRepos.FindQuery().Include(f => f.PlayerSession);

            playerQuery = playerQuery.Where(x => x.AgentId == clientAgent.AgentId);

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                playerQuery = playerQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                playerQuery = playerQuery.Where(x => x.State == model.State.Value);
            }

            return new GetAgentCreditBalanceResult
            {
                AgentCreditBalances = await playerQuery.OrderBy(x => x.State).ThenBy(x => x.Username).Select(x => new AgentCreditDto
                {
                    Id = x.PlayerId,
                    Credit = x.Credit,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MemberMaxCredit = null,
                    ParentId = null,
                    RoleId = Role.Player.ToInt(),
                    State = x.ParentState.HasValue && x.State >= x.ParentState.Value ? x.State : x.ParentState ?? x.State,
                    Username = x.Username,
                    Outstanding = 0m, //TODO: Implement later
                    Payout = 0m, //TODO: Implement later
                    Point = 0, //TODO: Implement later
                    Winlose = 0m, //TODO: Implement later
                    YesterdayPayout = 0m, //TODO: Implement later
                    YesterdayPoint = 0, //TODO: Implement later
                    YesterdayWinlose = 0m, //TODO: Implement later
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.PlayerSession != null ? x.PlayerSession.IpAddress : null,
                    LastLogin = x.PlayerSession != null ? x.PlayerSession.LatestDoingTime : null,
                    Platform = x.PlayerSession != null ? x.PlayerSession.Platform : null
                }).ToListAsync()
            };
        }

        private async Task<GetAgentCreditBalanceResult> GetAgentCreditBalance(IAgentRepository agentRepos, Data.Entities.Agent clientAgent, GetAgentCreditBalanceModel model)
        {
            var agentQuery = agentRepos.FindQuery().Include(f => f.AgentSession).AsQueryable();

            switch (clientAgent.RoleId.ToEnum<Role>())
            {
                case Role.Company:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Supermaster.ToInt() && x.ParentId == 0L);
                    break;
                case Role.Supermaster:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Master.ToInt() && x.SupermasterId == clientAgent.AgentId && x.ParentId == 0L);
                    break;
                case Role.Master:
                    agentQuery = agentQuery.Where(x => x.RoleId == Role.Agent.ToInt() && x.SupermasterId == clientAgent.SupermasterId && x.MasterId == clientAgent.AgentId && x.ParentId == 0L);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                agentQuery = agentQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                agentQuery = agentQuery.Where(x => x.State == model.State.Value);
            }

            return new GetAgentCreditBalanceResult
            {
                AgentCreditBalances = await agentQuery.OrderBy(x => x.State).ThenBy(x => x.Username).Select(x => new AgentCreditDto
                {
                    Id = x.AgentId,
                    Credit = x.Credit,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MemberMaxCredit = x.MemberMaxCredit,
                    ParentId = x.ParentId,
                    RoleId = x.RoleId,
                    State = x.ParentState.HasValue && x.State >= x.ParentState.Value ? x.State : x.ParentState ?? x.State,
                    Username = x.Username,
                    Outstanding = 0m, //TODO: Implement later
                    Payout = 0m, //TODO: Implement later
                    Point = 0, //TODO: Implement later
                    Winlose = 0m, //TODO: Implement later
                    YesterdayPayout = 0m, //TODO: Implement later
                    YesterdayPoint = 0, //TODO: Implement later
                    YesterdayWinlose = 0m, //TODO: Implement later
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.AgentSession != null ? x.AgentSession.IpAddress : null,
                    LastLogin = x.AgentSession != null ? x.AgentSession.LatestDoingTime : null,
                    Platform = x.AgentSession != null ? x.AgentSession.Platform : null
                }).ToListAsync()
            };
        }

        public async Task UpdateAgentCreditBalance(UpdateAgentCreditBalanceModel updateItem)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var updatedAgent = await agentRepos.FindByIdAsync(updateItem.AgentId) ?? throw new NotFoundException();

            decimal totalCreditUsedOfTargetAgent = updatedAgent.RoleId == Role.Agent.ToInt() ? await playerRepos.FindQueryBy(x => x.AgentId == updatedAgent.AgentId).SumAsync(x => x.Credit)
                                                                           : await agentRepos.SumAllCreditByTypeIdAsync(updatedAgent.AgentId, (Role)updatedAgent.RoleId);
            decimal maxCreditToCompare = 0m;
            switch (updatedAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    maxCreditToCompare = updatedAgent.Credit > 1000000
                                            ? updatedAgent.Credit < 1000000000
                                                ? updatedAgent.Credit * 100
                                                : updatedAgent.Credit * 5
                                            : 5000000;
                    break;
                case (int)Role.Master:
                    var supermaster = await agentRepos.FindByIdAsync(updatedAgent.SupermasterId) ?? throw new NotFoundException();
                    var totalSupermasterCreditUsed = await agentRepos.SumAllCreditByTypeIdAsync(supermaster.AgentId, (Role)supermaster.RoleId);
                    maxCreditToCompare = supermaster.Credit - totalSupermasterCreditUsed + updatedAgent.Credit;
                    break;
                case (int)Role.Agent:
                    var master = await agentRepos.FindByIdAsync(updatedAgent.MasterId) ?? throw new NotFoundException();
                    var totalMasterCreditUsed = await agentRepos.SumAllCreditByTypeIdAsync(master.AgentId, (Role)master.RoleId);
                    maxCreditToCompare = master.Credit - totalMasterCreditUsed + updatedAgent.Credit;
                    break;
                default:
                    maxCreditToCompare = 0m;
                    break;
            }

            if (updateItem.Credit < totalCreditUsedOfTargetAgent || updateItem.Credit > maxCreditToCompare)
                throw new BadRequestException(ErrorCodeHelper.Agent.InvalidCredit);
            var oldCreditValue = updatedAgent.Credit;
            updatedAgent.Credit = updateItem.Credit;
            updatedAgent.UpdatedAt = ClockService.GetUtcNow();
            updatedAgent.UpdatedBy = ClientContext.Agent.AgentId;
            agentRepos.Update(updatedAgent);

            await LotteryUow.SaveChangesAsync();

            await _auditService.SaveAuditData(new AuditParams
            {
                Type = (int)AuditType.Credit,
                EditedUsername = ClientContext.Agent.UserName,
                AgentUserName = updatedAgent.Username,
                AgentFirstName = updatedAgent.FirstName,
                AgentLastName = updatedAgent.LastName,
                Action = AuditDataHelper.Credit.Action.ActionUpdateAgentCredit,
                DetailMessage = string.Format(AuditDataHelper.Credit.DetailMessage.DetailUpdateAgentCredit, updatedAgent.Username),
                OldValue = oldCreditValue,
                NewValue = updatedAgent.Credit,
                SupermasterId = GetAuditSupermasterId(updatedAgent),
                MasterId = GetAuditMasterId(updatedAgent)
            });
        }

        public async Task<GetCreditBalanceDetailPopupResult> GetCreditBalanceDetailPopup(long agentId)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var targetAgent = await agentRepos.FindByIdAsync(agentId) ?? throw new NotFoundException();

            decimal totalCreditUsedOfTargetAgent = targetAgent.RoleId == Role.Agent.ToInt() ? await playerRepos.FindQueryBy(x => x.AgentId == targetAgent.AgentId).SumAsync(x => x.Credit)
                                                                           : await agentRepos.SumAllCreditByTypeIdAsync(targetAgent.AgentId, (Role)targetAgent.RoleId);
            decimal maxCreditToCompare = 0m;
            decimal minMemberMaxCredit = 0m;
            switch (targetAgent.RoleId)
            {
                case (int)Role.Supermaster:
                    maxCreditToCompare = targetAgent.Credit > 1000000
                                            ? targetAgent.Credit < 1000000000
                                                ? targetAgent.Credit * 100
                                                : targetAgent.Credit * 5
                                            : 5000000;
                    break;
                case (int)Role.Master:
                    var supermaster = await agentRepos.FindByIdAsync(targetAgent.SupermasterId) ?? throw new NotFoundException();
                    var totalSupermasterCreditUsed = await agentRepos.SumAllCreditByTypeIdAsync(supermaster.AgentId, (Role)supermaster.RoleId);
                    maxCreditToCompare = supermaster.Credit - totalSupermasterCreditUsed + targetAgent.Credit;
                    break;
                case (int)Role.Agent:
                    var master = await agentRepos.FindByIdAsync(targetAgent.MasterId) ?? throw new NotFoundException();
                    var totalMasterCreditUsed = await agentRepos.SumAllCreditByTypeIdAsync(master.AgentId, (Role)master.RoleId);
                    maxCreditToCompare = master.Credit - totalMasterCreditUsed + targetAgent.Credit;
                    var greatestCreditPlayer = await playerRepos.FindQueryBy(x => x.AgentId == targetAgent.AgentId).OrderByDescending(x => x.Credit).FirstOrDefaultAsync();
                    minMemberMaxCredit = greatestCreditPlayer?.Credit ?? 0m;
                    break;
                default:
                    maxCreditToCompare = 0m;
                    break;
            }
            return new GetCreditBalanceDetailPopupResult
            {
                CurrentGivenCredit = targetAgent.Credit,
                GivenCredit = targetAgent.Credit,
                MinCredit = totalCreditUsedOfTargetAgent,
                MaxCredit = maxCreditToCompare,
                MinMemberMaxCredit = minMemberMaxCredit
            };
        }

        public async Task<List<SearchAgentDto>> SearchAgent(string searchTerm)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();

            var agentQuery = await agentRepos.FindQueryBy(x => x.RoleId != Role.Company.ToInt() && x.Username.Contains(searchTerm)).Select(x => new SearchAgentDto
            {
                TargetId = x.AgentId,
                Username = x.Username,
                IsAgent = true
            }).ToListAsync();
            var playerQuery = await playerRepos.FindQueryBy(x => x.Username.Contains(searchTerm)).Select(x => new SearchAgentDto
            {
                TargetId = x.PlayerId,
                Username = x.Username,
                IsAgent = false
            }).ToListAsync();

            return agentQuery.Concat(playerQuery).OrderBy(x => x.Username).ToList();
        }
    }
}
