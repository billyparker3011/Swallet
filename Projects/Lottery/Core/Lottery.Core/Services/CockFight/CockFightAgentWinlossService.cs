using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Agent.GetAgentWinLossSummary;
using Lottery.Core.Models.CockFight.GetCockFightAgentWinloss;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.CockFight;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.CockFight
{
    public class CockFightAgentWinlossService : LotteryBaseService<CockFightAgentWinlossService>, ICockFightAgentWinlossService
    {
        public CockFightAgentWinlossService(ILogger<CockFightAgentWinlossService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinloss(long? agentId, DateTime from, DateTime to)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var cockFightTicketRepos = LotteryUow.GetRepository<ICockFightTicketRepository>();
            var targetAgentId = agentId.HasValue ? agentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var loginAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var ticketStates = CommonHelper.CompletedCockFightTicketWithoutRefundOrRejectState();
            switch (loginAgent.RoleId)
            {
                case (int)Role.Company:
                    return await GetCockFightAgentWinLossOfCompany(agentRepos, cockFightTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Supermaster:
                    return await GetCockFightAgentWinLossOfSupermaster(agentRepos, cockFightTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Master:
                    return await GetCockFightAgentWinLossOfMaster(agentRepos, cockFightTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Agent:
                    return await GetCockFightAgentWinLossOfAgent(playerRepos, cockFightTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                default:
                    return new GetCockFightAgentWinLossSummaryResult();
            };
        }

        private async Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinLossOfAgent(IPlayerRepository playerRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var cockFightAgentWinlossSummaries = await playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(cockFightTicketRepos.FindQueryBy(y => ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
                                   x => x.PlayerId, y => y.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       ticket.ParentId,
                                       ticket.WinlossAmount,
                                       ticket.BetAmount,
                                       ticket.ShowMore,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username
                                   })
                                   .Select(x => new CockFightAgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       RoleId = Role.Player.ToInt(),
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId <= Role.Agent.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCockFightAgentWinLossSummaryResult
            {
                CockFightAgentWinlossSummaries = cockFightAgentWinlossSummaries,
                TotalBetCount = cockFightAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = cockFightAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = cockFightAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCockFightAgentWinLoseInfo = new List<TotalCockFightAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    }
                },
                TotalCompany = cockFightAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinLossOfMaster(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentWinlossSummaries = await agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQueryBy(y => ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.WinlossAmount,
                                       ticket.BetAmount,
                                       ticket.ShowMore,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m,
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId <= Role.Master.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCockFightAgentWinLossSummaryResult
            {
                CockFightAgentWinlossSummaries = cockFightAgentWinlossSummaries,
                TotalBetCount = cockFightAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = cockFightAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = cockFightAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCockFightAgentWinLoseInfo = new List<TotalCockFightAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    }
                },
                TotalCompany = cockFightAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinLossOfSupermaster(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentWinlossSummaries = await agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQueryBy(y => ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.WinlossAmount,
                                       ticket.BetAmount,
                                       ticket.ShowMore,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId <= Role.Supermaster.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCockFightAgentWinLossSummaryResult
            {
                CockFightAgentWinlossSummaries = cockFightAgentWinlossSummaries,
                TotalBetCount = cockFightAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = cockFightAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = cockFightAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCockFightAgentWinLoseInfo = new List<TotalCockFightAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalCompany = cockFightAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinLossOfCompany(IAgentRepository agentRepos, ICockFightTicketRepository cockFightTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var cockFightAgentWinlossSummaries = await agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(cockFightTicketRepos.FindQueryBy(y => ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
                                   x => x.AgentId, y => y.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.ParentId,
                                       ticket.WinlossAmount,
                                       ticket.BetAmount,
                                       ticket.ShowMore,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId
                                   })
                                   .Select(x => new CockFightAgentWinlossSummaryDto
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CockFightWinlossInfo
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCockFightAgentWinLossSummaryResult
            {
                CockFightAgentWinlossSummaries = cockFightAgentWinlossSummaries,
                TotalCockFightAgentWinLoseInfo = new List<TotalCockFightAgentWinLoseInfo>
                {
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = cockFightAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalBetCount = cockFightAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = cockFightAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = cockFightAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCompany = cockFightAgentWinlossSummaries.Sum(x => x.Company)
            };
        }
    }
}
