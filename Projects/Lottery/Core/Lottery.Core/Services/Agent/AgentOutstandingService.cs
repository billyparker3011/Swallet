using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentOuts;
using Lottery.Core.Models.Agent.GetAgentOutstanding;
using Lottery.Core.Repositories.Agent;
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
                                                : ClientContext.Agent.ParentId == 0L
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
                    listAgentOustanding = await GetAgentOutstandingsOfSupermaster(agentRepos, ticketRepos, targetAgentId, targetRoleId, model, outsState);
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
            var agentOutsQuery = playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(ticketRepos.FindQuery().Where(y => outsState.Contains(y.State)), x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       ticket.ParentId,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalStake = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Stake),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerPayout)
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
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(ticketRepos.FindQuery().Where(y => outsState.Contains(y.State)), x => x.AgentId, y => y.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalStake = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Stake),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerPayout)
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

        private async Task<List<AgentOutstandingDto>> GetAgentOutstandingsOfSupermaster(IAgentRepository agentRepos, ITicketRepository ticketRepos, long targetAgentId, int targetRoleId, GetAgentOutstandingModel model, List<int> outsState)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(ticketRepos.FindQuery().Where(y => outsState.Contains(y.State)), x => x.AgentId, y => y.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalStake = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Stake),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerPayout)
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
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var agentOutsQuery = agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(ticketRepos.FindQuery().Where(y => outsState.Contains(y.State)), x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.Stake,
                                       ticket.PlayerPayout,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new AgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalStake = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Stake),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.PlayerPayout)
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
    }
}
