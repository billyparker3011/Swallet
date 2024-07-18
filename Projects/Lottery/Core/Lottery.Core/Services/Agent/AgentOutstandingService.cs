using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.BroadCaster;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentOuts;
using Lottery.Core.Models.Agent.GetAgentOutstanding;
using Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.BetKind;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lottery.Core.Services.Agent
{
    public class AgentOutstandingService : LotteryBaseService<AgentOutstandingService>, IAgentOutstandingService
    {
        public AgentOutstandingService(ILogger<AgentOutstandingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetAgentOutstandingResult> GetAgentOutstandings(GetAgentOutstandingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var ticketRepos = LotteryUow.GetRepository<ITicketRepository>();
            var listAgentOustanding = new List<AgentOutstandingDto>();
            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                : ClientContext.Agent.ParentId == 0
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var targetRoleId = model.RoleId.HasValue ? model.RoleId.Value : ClientContext.Agent.RoleId;
            if (targetRoleId < ClientContext.Agent.RoleId) return new GetAgentOutstandingResult();
            var outsState = CommonHelper.OutsTicketState();
            switch (targetRoleId)
            {
                case (int)Role.Company:
                    listAgentOustanding = await GetAgentOutstandingsOfCompany(agentRepos, ticketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Supermaster:
                    listAgentOustanding = await GetAgentOutstandingsOfSuperMaster(agentRepos, ticketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Master:
                    listAgentOustanding = await GetAgentOutstandingsOfMaster(agentRepos, ticketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Agent:
                    listAgentOustanding = await GetAgentOutstandingsOfAgent(playerRepos, ticketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                default:
                    break;
            }

            return new GetAgentOutstandingResult
            {
                AgentOuts = listAgentOustanding,
                SummaryBetCount = listAgentOustanding.Sum(x => x.TotalBetCount),
                SummaryStake = listAgentOustanding.Sum(x => x.TotalStake),
                SummaryPayout = listAgentOustanding.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<AgentOutstandingDto>> GetAgentOutstandingsOfAgent(IPlayerRepository playerRepos, ITicketRepository ticketRepos, long targetAgentId, int targetRoleId, GetAgentOutstandingModel model, List<int> outsState)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var agentOutsQuery = playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId)).Include(x => x.PlayerSession)
                                   .Join(ticketRepos.FindQuery().Where(y => !y.ParentId.HasValue && outsState.Contains(y.State)), x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       player.PlayerSession.IpAddress,
                                       player.PlayerSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.LongCount(),
                                       TotalStake = x.Sum(s => s.Stake),
                                       TotalPayout = x.Sum(s => s.PlayerPayout),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                agentOutsQuery = agentOutsQuery.OrderByDescending(GetSortAgentOutsProperty(model));
            }
            else
            {
                agentOutsQuery = agentOutsQuery.OrderBy(GetSortAgentOutsProperty(model));
            }

            return await agentOutsQuery.ToListAsync();
        }

        private async Task<List<AgentOutstandingDto>> GetAgentOutstandingsOfMaster(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, int targetRoleId, GetAgentOutstandingModel model, List<int> outsState)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQuery().Where(y => !y.ParentId.HasValue && outsState.Contains(y.State)), x => x.AgentId, y => y.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(),
                                       TotalStake = x.Sum(s => s.Stake),
                                       TotalPayout = x.Sum(s => s.PlayerPayout),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                agentOutsQuery = agentOutsQuery.OrderByDescending(GetSortAgentOutsProperty(model));
            }
            else
            {
                agentOutsQuery = agentOutsQuery.OrderBy(GetSortAgentOutsProperty(model));
            }

            return await agentOutsQuery.ToListAsync();
        }

        private async Task<List<AgentOutstandingDto>> GetAgentOutstandingsOfSuperMaster(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, int targetRoleId, GetAgentOutstandingModel model, List<int> outsState)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQuery().Where(y => !y.ParentId.HasValue && outsState.Contains(y.State)), x => x.AgentId, y => y.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(),
                                       TotalStake = x.Sum(s => s.Stake),
                                       TotalPayout = x.Sum(s => s.PlayerPayout),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                agentOutsQuery = agentOutsQuery.OrderByDescending(GetSortAgentOutsProperty(model));
            }
            else
            {
                agentOutsQuery = agentOutsQuery.OrderBy(GetSortAgentOutsProperty(model));
            }

            return await agentOutsQuery.ToListAsync();
        }

        private async Task<List<AgentOutstandingDto>> GetAgentOutstandingsOfCompany(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, int targetRoleId, GetAgentOutstandingModel model, List<int> outsState)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == targetRoleId + 1 && x.ParentId == 0).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId)).Include(x => x.AgentSession)
                                   .Join(ticketRepos.FindQuery().Where(y => !y.ParentId.HasValue && outsState.Contains(y.State)), x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       agent.AgentSession.IpAddress,
                                       agent.AgentSession.Platform,
                                       ticket.Stake,
                                       ticket.PlayerPayout
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.IpAddress,
                                       x.Platform
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(),
                                       TotalStake = x.Sum(s => s.Stake),
                                       TotalPayout = x.Sum(s => s.PlayerPayout),
                                       IpAddress = x.Key.IpAddress,
                                       Platform = x.Key.Platform
                                   }).AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                agentOutsQuery = agentOutsQuery.OrderByDescending(GetSortAgentOutsProperty(model));
            }
            else
            {
                agentOutsQuery = agentOutsQuery.OrderBy(GetSortAgentOutsProperty(model));
            }

            return await agentOutsQuery.ToListAsync();
        }

        private Expression<Func<AgentOutstandingDto, object>> GetSortAgentOutsProperty(GetAgentOutstandingModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return agentOuts => agentOuts.Username;
            return model.SortName?.ToLower() switch
            {
                "totalbetcount" => agentOuts => agentOuts.TotalBetCount,
                "totalstake" => agentOuts => agentOuts.TotalStake,
                "totalpayout" => agentOuts => agentOuts.TotalPayout,
                "username" => agentOuts => agentOuts.Username,
                _ => agentOuts => agentOuts.Username
            };
        }

        public async Task<GetBroadCasterOutstandingResult> GetBroadCasterOutstandings(GetBroadCasterOutstandingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var betKindRepos = LotteryUow.GetRepository<IBetKindRepository>();
            var ticketRepos = LotteryUow.GetRepository<ITicketRepository>();
            var listBroadCasterOustanding = new List<BroadCasterOutstandingDto>();
            var targetAgentId =  ClientContext.Agent.ParentId == 0
                                        ? ClientContext.Agent.AgentId
                                        : ClientContext.Agent.ParentId;
            var outsState = CommonHelper.OutsTicketState();
            switch (ClientContext.Agent.RoleId)
            {
                case (int)Role.Company:
                    var companyTicketQuery = ticketRepos.FindQueryBy(tk => !tk.ParentId.HasValue && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(companyTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Supermaster:
                    var superMasterTicketQuery = ticketRepos.FindQueryBy(tk => tk.SupermasterId == targetAgentId && !tk.ParentId.HasValue && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(superMasterTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Master:
                    var masterTicketQuery = ticketRepos.FindQueryBy(tk => tk.MasterId == targetAgentId && !tk.ParentId.HasValue && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(masterTicketQuery, betKindRepos, model);
                    break;
                case (int)Role.Agent:
                    var agentTicketQuery = ticketRepos.FindQueryBy(tk => tk.AgentId == targetAgentId && !tk.ParentId.HasValue && outsState.Contains(tk.State));
                    listBroadCasterOustanding = await GetBroadCasterOutstandings(agentTicketQuery, betKindRepos, model);
                    break;
                default:
                    break;
            }

            return new GetBroadCasterOutstandingResult
            {
                BroadCasterOuts = listBroadCasterOustanding,
                SummaryBetCount = listBroadCasterOustanding.Sum(x => x.TotalBetCount),
                SummaryStake = listBroadCasterOustanding.Sum(x => x.TotalStake),
                SummaryPayout = listBroadCasterOustanding.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<BroadCasterOutstandingDto>> GetBroadCasterOutstandings(IQueryable<Data.Entities.Ticket> ticketQuery, IBetKindRepository betKindRepos, GetBroadCasterOutstandingModel model)
        {
            var broadCasterOutsQuery = ticketQuery
                 .Join(betKindRepos.FindQuery(),
                 tk => new { tk.RegionId, tk.BetKindId },
                 bk => new { bk.RegionId, BetKindId = bk.Id },
                 (tk, bk) => new { bk.RegionId, bk.CategoryId, bk.Id, bk.Name, tk.Stake, tk.PlayerPayout })
                 .GroupBy(joinedData => new { joinedData.RegionId, joinedData.CategoryId, joinedData.Id, joinedData.Name })
                 .Select(groupedData => new BroadCasterOutstandingDto
                 {
                     RegionId = groupedData.Key.RegionId,
                     CategoryId = groupedData.Key.CategoryId,
                     BetKindId = groupedData.Key.Id,
                     BetKindName = groupedData.Key.Name,
                     TotalStake = groupedData.Sum(x => x.Stake),
                     TotalPayout = groupedData.Sum(x => x.PlayerPayout),
                     TotalBetCount = groupedData.Count()
                 });

            if (model.SortType == SortType.Descending)
            {
                if (string.IsNullOrEmpty(model.SortName) || model.SortName.ToLower() == "betkind")
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderByDescending(x => x.RegionId).ThenByDescending(x => x.CategoryId).ThenByDescending(x => x.BetKindId);
                }
                else
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderByDescending(GetSortBroadCasterOutsProperty(model));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.SortName) || model.SortName.ToLower() == "betkind")
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderBy(x => x.RegionId).ThenBy(x => x.CategoryId).ThenBy(x => x.BetKindId);
                }
                else
                {
                    broadCasterOutsQuery = broadCasterOutsQuery.OrderBy(GetSortBroadCasterOutsProperty(model));
                }
            }
            var result = await broadCasterOutsQuery.ToListAsync();
            foreach (var item in result)
            {
                item.RegionName = EnumCategoryHelper.GetEnumRegionInformation((Region)item.RegionId)?.Name;
                item.CategoryName = EnumCategoryHelper.GetEnumCategoryInformation((Category)item.CategoryId)?.Name;
            }
            return result;
        }

        private Expression<Func<BroadCasterOutstandingDto, object>> GetSortBroadCasterOutsProperty(GetBroadCasterOutstandingModel model)
        {
            return model.SortName?.ToLower() switch
            {
                "totalbetcount" => broadCasterOuts => broadCasterOuts.TotalBetCount,
                "totalstake" => broadCasterOuts => broadCasterOuts.TotalStake,
                "totalpayout" => broadCasterOuts => broadCasterOuts.TotalPayout
            };
        }
    }
}
