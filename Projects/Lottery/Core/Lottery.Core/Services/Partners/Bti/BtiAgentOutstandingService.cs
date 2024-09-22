using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiAgentOutstandingService : LotteryBaseService<BtiAgentOutstandingService>, IBtiAgentOutstandingService
    {
        public BtiAgentOutstandingService(ILogger<BtiAgentOutstandingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<BtiAgentOutstandingResultModel> GetBtiAgentOutstanding(GetBtiAgentOutstandingModel model)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var listBtiAgentOustanding = new List<BtiAgentOutstandingModel>();
            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var targetRoleId = model.RoleId.HasValue ? model.RoleId.Value : ClientContext.Agent.RoleId;
            if (targetRoleId < ClientContext.Agent.RoleId) return new BtiAgentOutstandingResultModel();
            var outsState = BtiTicketStatusHelper.Betting;
            var outsType = new List<int> { BtiTypeHelper.Reverse };
            switch (targetRoleId)
            {
                case (int)Role.Company:
                    listBtiAgentOustanding = await GetBtiAgentOutstandingsOfCompany(agentRepos, btiTicketRepos, targetAgentId, targetRoleId, model, outsState, outsType);
                    break;
                case (int)Role.Supermaster:
                    listBtiAgentOustanding = await GetBtiAgentOutstandingsOfSupermaster(agentRepos, btiTicketRepos, targetAgentId, targetRoleId, model, outsState, outsType);
                    break;
                case (int)Role.Master:
                    listBtiAgentOustanding = await GetBtiAgentOutstandingsOfMaster(agentRepos, btiTicketRepos, targetAgentId, targetRoleId, model, outsState, outsType);
                    break;
                case (int)Role.Agent:
                    listBtiAgentOustanding = await GetBtiAgentOutstandingsOfAgent(playerRepos, btiTicketRepos, targetAgentId, targetRoleId, model, outsState, outsType);
                    break;
                default:
                    break;
            }

            return new BtiAgentOutstandingResultModel
            {
                BtiAgentOuts = listBtiAgentOustanding,
                SummaryBetCount = listBtiAgentOustanding.Sum(x => x.TotalBetCount),
                SummaryPayout = listBtiAgentOustanding.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<BtiAgentOutstandingModel>> GetBtiAgentOutstandingsOfAgent(IPlayerRepository playerRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, int targetRoleId, GetBtiAgentOutstandingModel model, List<int> outsState, List<int> outsType)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var btiAgentOutsQuery = playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(btiTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status) && outsType.Contains(y.Type)), x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
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
                                   .Select(x => new BtiAgentOutstandingModel
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
                btiAgentOutsQuery = btiAgentOutsQuery.OrderByDescending(GetSortBtiAgentOutsProperty(model));
            }
            else
            {
                btiAgentOutsQuery = btiAgentOutsQuery.OrderBy(GetSortBtiAgentOutsProperty(model));
            }

            return await btiAgentOutsQuery.ToListAsync();
        }

        private async Task<List<BtiAgentOutstandingModel>> GetBtiAgentOutstandingsOfMaster(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, int targetRoleId, GetBtiAgentOutstandingModel model, List<int> outsState, List<int> outsType)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var btiAgentOutsQuery = agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status) && outsType.Contains(y.Type)), x => x.AgentId, y => y.AgentId, (agent, ticket) => new
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
                                   .Select(x => new BtiAgentOutstandingModel
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
                btiAgentOutsQuery = btiAgentOutsQuery.OrderByDescending(GetSortBtiAgentOutsProperty(model));
            }
            else
            {
                btiAgentOutsQuery = btiAgentOutsQuery.OrderBy(GetSortBtiAgentOutsProperty(model));
            }

            return await btiAgentOutsQuery.ToListAsync();
        }

        private async Task<List<BtiAgentOutstandingModel>> GetBtiAgentOutstandingsOfSupermaster(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, int targetRoleId, GetBtiAgentOutstandingModel model, List<int> outsState, List<int> outsType)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var btiAgentOutsQuery = agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status) && outsType.Contains(y.Type)), x => x.AgentId, y => y.MasterId, (agent, ticket) => new
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
                                   .Select(x => new BtiAgentOutstandingModel
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
                btiAgentOutsQuery = btiAgentOutsQuery.OrderByDescending(GetSortBtiAgentOutsProperty(model));
            }
            else
            {
                btiAgentOutsQuery = btiAgentOutsQuery.OrderBy(GetSortBtiAgentOutsProperty(model));
            }

            return await btiAgentOutsQuery.ToListAsync();
        }

        private async Task<List<BtiAgentOutstandingModel>> GetBtiAgentOutstandingsOfCompany(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, int targetRoleId, GetBtiAgentOutstandingModel model, List<int> outsState, List<int> outsType)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var btiAgentOutsQuery = agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQuery().Where(y => outsState.Contains(y.Status) && outsType.Contains(y.Type)), x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
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
                                   .Select(x => new BtiAgentOutstandingModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = (Role)x.Key.RoleId,
                                       TotalBetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       TotalPayout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.Payout)
                                   }).AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                btiAgentOutsQuery = btiAgentOutsQuery.OrderByDescending(GetSortBtiAgentOutsProperty(model));
            }
            else
            {
                btiAgentOutsQuery = btiAgentOutsQuery.OrderBy(GetSortBtiAgentOutsProperty(model));
            }

            return await btiAgentOutsQuery.ToListAsync();
        }

        private Expression<Func<BtiAgentOutstandingModel, object>> GetSortBtiAgentOutsProperty(GetBtiAgentOutstandingModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return BtiAgentOuts => BtiAgentOuts.Username;
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
