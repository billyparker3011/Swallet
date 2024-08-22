using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.CockFight.GetCockFightAgentOutstanding;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentOutstandingService : LotteryBaseService<CockFightAgentOutstandingService>, ICockFightAgentOutstandingService
    {
        public CockFightAgentOutstandingService(ILogger<CockFightAgentOutstandingService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ILotteryClientContext clientContext, 
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetCockFightAgentOutstandingResult> GetCockFightAgentOutstanding(GetCockFightAgentOutstandingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightTicketRepos = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var listCockFightAgentOustanding = new List<CockFightAgentOutstandingDto>();
            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var targetRoleId = model.RoleId.HasValue ? model.RoleId.Value : ClientContext.Agent.RoleId;
            if (targetRoleId < ClientContext.Agent.RoleId) return new GetCockFightAgentOutstandingResult();
            var outsState = CommonHelper.OutsCockFightTicketState();
            switch (targetRoleId)
            {
                case (int)Role.Company:
                    listCockFightAgentOustanding = await GetCockFightAgentOutstandingsOfCompany(agentRepos, cockFightTicketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Supermaster:
                    listCockFightAgentOustanding = await GetCockFightAgentOutstandingsOfSupermaster(agentRepos, cockFightTicketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Master:
                    listCockFightAgentOustanding = await GetCockFightAgentOutstandingsOfMaster(agentRepos, cockFightTicketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                case (int)Role.Agent:
                    listCockFightAgentOustanding = await GetCockFightAgentOutstandingsOfAgent(playerRepos, cockFightTicketRepos, targetAgentId, targetRoleId, model, outsState);
                    break;
                default:
                    break;
            }

            return new GetCockFightAgentOutstandingResult
            {
                CockFightAgentOuts = listCockFightAgentOustanding,
                SummaryBetCount = listCockFightAgentOustanding.Sum(x => x.TotalBetCount),
                SummaryPayout = listCockFightAgentOustanding.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<CockFightAgentOutstandingDto>> GetCockFightAgentOutstandingsOfAgent(IPlayerRepository playerRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, int targetRoleId, GetCockFightAgentOutstandingModel model, List<int> outsState)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var cockFightAgentOutsQuery = playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(cockFightTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status)), x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       ticket.ParentId,
                                       Payout = ticket.TicketAmount ?? 0m,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username
                                   })
                                   .Select(x => new CockFightAgentOutstandingDto
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await cockFightAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CockFightAgentOutstandingDto>> GetCockFightAgentOutstandingsOfMaster(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, int targetRoleId, GetCockFightAgentOutstandingModel model, List<int> outsState)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentOutsQuery = agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status)), x => x.AgentId, y => y.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       Payout = ticket.TicketAmount ?? 0m,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await cockFightAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CockFightAgentOutstandingDto>> GetCockFightAgentOutstandingsOfSupermaster(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, int targetRoleId, GetCockFightAgentOutstandingModel model, List<int> outsState)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentOutsQuery = agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status)), x => x.AgentId, y => y.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       Payout = ticket.TicketAmount ?? 0m,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await cockFightAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CockFightAgentOutstandingDto>> GetCockFightAgentOutstandingsOfCompany(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, int targetRoleId, GetCockFightAgentOutstandingModel model, List<int> outsState)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentOutsQuery = agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status)), x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       Payout = ticket.TicketAmount ?? 0m,
                                       ticket.ShowMore
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentOutstandingDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Payout)
                                   }).AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                cockFightAgentOutsQuery = cockFightAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await cockFightAgentOutsQuery.ToListAsync();
        }

        private Expression<Func<CockFightAgentOutstandingDto, object>> GetSortCockFightAgentOutsProperty(GetCockFightAgentOutstandingModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return cockFightAgentOuts => cockFightAgentOuts.Username;
            return model.SortName?.ToLower() switch
            {
                "totalbetcount" => agentOuts => agentOuts.TotalBetCount,
                "totalpayout" => agentOuts => agentOuts.TotalPayout,
                "username" => agentOuts => agentOuts.Username,
                _ => agentOuts => agentOuts.Username
            };
        }
    }
}
