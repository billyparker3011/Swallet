using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
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
using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiAgentWinlossService : LotteryBaseService<BtiAgentWinlossService>, IBtiAgentWinlossService
    {
        public BtiAgentWinlossService(ILogger<BtiAgentWinlossService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinloss(long? agentId, DateTime from, DateTime to)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var btiTicketRepos = LotteryUow.GetRepository<IBtiTicketRepository>();
            var targetAgentId = agentId.HasValue ? agentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var loginAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var ticketStates = BtiTicketStatusHelper.Betted;
            switch (loginAgent.RoleId)
            {
                case (int)Role.Company:
                    return await GetBtiAgentWinLossOfCompany(agentRepos, btiTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Supermaster:
                    return await GetBtiAgentWinLossOfSupermaster(agentRepos, btiTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Master:
                    return await GetBtiAgentWinLossOfMaster(agentRepos, btiTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Agent:
                    return await GetBtiAgentWinLossOfAgent(playerRepos, btiTicketRepos, targetAgentId, loginAgent, from, to, ticketStates);
                default:
                    return new GetBtiAgentWinLossSummaryResultModel();
            };
        }

        private async Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinLossOfAgent(IPlayerRepository playerRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var BtiAgentWinlossSummaries = await playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(btiTicketRepos.FindQueryBy(y => y.Status != BtiTicketStatusHelper.Cancel && ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
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
                                   .Select(x => new BtiAgentWinLossModel
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       RoleId = Role.Player.ToInt(),
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId <= Role.Agent.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetBtiAgentWinLossSummaryResultModel
            {
                BtiAgentWinlossSummaries = BtiAgentWinlossSummaries,
                TotalBetCount = BtiAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = BtiAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = BtiAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalBtiAgentWinLoseInfo = new List<TotalBtiAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    }
                },
                TotalCompany = BtiAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinLossOfMaster(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var BtiAgentWinlossSummaries = await agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQueryBy(y => y.Status != BtiTicketStatusHelper.Cancel && ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
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
                                   .Select(x => new BtiAgentWinLossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m,
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId <= Role.Master.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetBtiAgentWinLossSummaryResultModel
            {
                BtiAgentWinlossSummaries = BtiAgentWinlossSummaries,
                TotalBetCount = BtiAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = BtiAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = BtiAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalBtiAgentWinLoseInfo = new List<TotalBtiAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    }
                },
                TotalCompany = BtiAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinLossOfSupermaster(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var BtiAgentWinlossSummaries = await agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQueryBy(y => y.Status != BtiTicketStatusHelper.Cancel && ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
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
                                   .Select(x => new BtiAgentWinLossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId <= Role.Supermaster.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetBtiAgentWinLossSummaryResultModel
            {
                BtiAgentWinlossSummaries = BtiAgentWinlossSummaries,
                TotalBetCount = BtiAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = BtiAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = BtiAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalBtiAgentWinLoseInfo = new List<TotalBtiAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalCompany = BtiAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinLossOfCompany(IAgentRepository agentRepos, IBtiTicketRepository btiTicketRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var BtiAgentWinlossSummaries = await agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(btiTicketRepos.FindQueryBy(y => y.Status != BtiTicketStatusHelper.Cancel && ticketStates.Contains(y.Status) && y.TicketModifiedDate >= from.Date && y.TicketModifiedDate <= to.AddDays(1).AddTicks(-1)),
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
                                   .Select(x => new BtiAgentWinLossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(f => (!f.ParentId.HasValue && !f.ShowMore.HasValue) || (!f.ParentId.HasValue && f.ShowMore.HasValue && !f.ShowMore.Value) || (f.ParentId.HasValue && !f.ShowMore.HasValue)),
                                       Payout = x.Where(f => !f.ParentId.HasValue).Sum(s => s.BetAmount ?? 0m),
                                       WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.WinlossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new BtiWinlossInfoModel
                                                       {
                                                           WinLose = x.Where(f => !f.ParentId.HasValue).Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Where(f => !f.ParentId.HasValue).Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetBtiAgentWinLossSummaryResultModel
            {
                BtiAgentWinlossSummaries = BtiAgentWinlossSummaries,
                TotalBtiAgentWinLoseInfo = new List<TotalBtiAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = BtiAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalBetCount = BtiAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = BtiAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = BtiAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCompany = BtiAgentWinlossSummaries.Sum(x => x.Company)
            };
        }
    }
}
