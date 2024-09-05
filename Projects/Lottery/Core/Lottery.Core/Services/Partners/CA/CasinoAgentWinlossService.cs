using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Lottery.Core.Helpers.PartnerHelper;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentWinlossService : LotteryBaseService<CasinoAgentWinlossService>, ICasinoAgentWinlossService
    {
        public CasinoAgentWinlossService(ILogger<CasinoAgentWinlossService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinloss(long? agentId, DateTime from, DateTime to)
        {
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepos = LotteryUow.GetRepository<IPlayerRepository>();
            var casinoTicketBetDetailRepos = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var targetAgentId = agentId.HasValue ? agentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var loginAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var ticketStates = PartnerHelper.CasinoBetStatus.BetCompleted.ToList();
            switch (loginAgent.RoleId)
            {
                case (int)Role.Company:
                    return await GetCasinoAgentWinLossOfCompany(agentRepos, casinoTicketBetDetailRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Supermaster:
                    return await GetCasinoAgentWinLossOfSupermaster(agentRepos, casinoTicketBetDetailRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Master:
                    return await GetCasinoAgentWinLossOfMaster(agentRepos, casinoTicketBetDetailRepos, targetAgentId, loginAgent, from, to, ticketStates);
                case (int)Role.Agent:
                    return await GetCasinoAgentWinLossOfAgent(playerRepos, casinoTicketBetDetailRepos, targetAgentId, loginAgent, from, to, ticketStates);
                default:
                    return new GetCasinoAgentWinLossSummaryResultModel();
            };
        }

        private async Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinLossOfAgent(IPlayerRepository playerRepos, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var playerIds = await playerRepos.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var queryTicketCompleted = casinoTicketBetDetailRepos.FindQueryBy(c => ticketStates.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoAgentWinlossSummaries = await playerRepos.FindQueryBy(x => playerIds.Contains(x.PlayerId))
                                   .Join(casinoTicketBetDetailRepos.FindQueryBy(y => queryTicketCompleted.Contains(y.GameRoundId) && !y.IsCancel && y.CreatedAt >= from.Date && y.CreatedAt <= to.AddDays(1).AddTicks(-1)).Include(c=>c.CasinoTicket),
                                   x => x.PlayerId, y => y.CasinoTicket.PlayerId, (player, ticket) => new
                                   {
                                       player.PlayerId,
                                       player.Username,
                                       ticket.GameRoundId,
                                       ticket.WinOrLossAmount,
                                       ticket.BetAmount,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss,
                                       ticket.Status,
                                       ticket.CasinoTicket.Type
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username,
                                       x.GameRoundId,
                                   })
                                   .Select(x => new CasinoAgentWinlossModel
                                   {
                                       PlayerId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       AgentId = targetAgentId,
                                       RoleId = Role.Player.ToInt(),
                                       BetCount = x.LongCount(),
                                       Payout = x.Sum(s => (s.Status == CasinoBetStatus.Settled && s.Type != CasinoTransferType.Manual_Settle) ? 0m : s.BetAmount),
                                       WinLose = x.Sum(s => s.WinOrLossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId <= Role.Agent.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.MasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCasinoAgentWinLossSummaryResultModel
            {
                CasinoAgentWinlossSummaries = casinoAgentWinlossSummaries,
                TotalBetCount = casinoAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = casinoAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = casinoAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCasinoAgentWinLoseInfo = new List<TotalCasinoAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    }
                },
                TotalCompany = casinoAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinLossOfMaster(IAgentRepository agentRepos, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var agentIds = await agentRepos.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var queryTicketCompleted = casinoTicketBetDetailRepos.FindQueryBy(c => ticketStates.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoAgentWinlossSummaries = await agentRepos.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetailRepos.FindQueryBy(y => queryTicketCompleted.Contains(y.GameRoundId) && !y.IsCancel && y.CreatedAt >= from.Date && y.CreatedAt <= to.AddDays(1).AddTicks(-1)).Include(c=>c.CasinoTicket),
                                   x => x.AgentId, y => y.CasinoTicket.AgentId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.GameRoundId,
                                       ticket.WinOrLossAmount,
                                       ticket.BetAmount,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss,
                                       ticket.Status,
                                       ticket.CasinoTicket.Type
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.GameRoundId
                                   })
                                   .Select(x => new CasinoAgentWinlossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(),
                                       Payout = x.Sum(s => (s.Status == CasinoBetStatus.Settled && s.Type != CasinoTransferType.Manual_Settle) ? 0m : s.BetAmount),
                                       WinLose = x.Sum(s => s.WinOrLossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = 0m,
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId <= Role.Master.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.SupermasterWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCasinoAgentWinLossSummaryResultModel
            {
                CasinoAgentWinlossSummaries = casinoAgentWinlossSummaries,
                TotalBetCount = casinoAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = casinoAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = casinoAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCasinoAgentWinLoseInfo = new List<TotalCasinoAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    }
                },
                TotalCompany = casinoAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinLossOfSupermaster(IAgentRepository agentRepos, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var masterIds = await agentRepos.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var queryTicketCompleted = casinoTicketBetDetailRepos.FindQueryBy(c => ticketStates.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoAgentWinlossSummaries = await agentRepos.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetailRepos.FindQueryBy(y => queryTicketCompleted.Contains(y.GameRoundId) && !y.IsCancel && y.CreatedAt >= from.Date && y.CreatedAt <= to.AddDays(1).AddTicks(-1)).Include(c=>c.CasinoTicket),
                                   x => x.AgentId, y => y.CasinoTicket.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.GameRoundId,
                                       ticket.WinOrLossAmount,
                                       ticket.BetAmount,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss,
                                       ticket.Status,
                                       ticket.CasinoTicket.Type
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.GameRoundId,
                                   })
                                   .Select(x => new CasinoAgentWinlossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(),
                                       Payout = x.Sum(s => (s.Status == CasinoBetStatus.Settled && s.Type != CasinoTransferType.Manual_Settle) ? 0m : s.BetAmount),
                                       WinLose = x.Sum(s => s.WinOrLossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId <= Role.Supermaster.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCasinoAgentWinLossSummaryResultModel
            {
                CasinoAgentWinlossSummaries = casinoAgentWinlossSummaries,
                TotalBetCount = casinoAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = casinoAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = casinoAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCasinoAgentWinLoseInfo = new List<TotalCasinoAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalCompany = casinoAgentWinlossSummaries.Sum(x => x.Company)
            };
        }

        private async Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinLossOfCompany(IAgentRepository agentRepos, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepos, long targetAgentId, Data.Entities.Agent loginAgent, DateTime from, DateTime to, List<int> ticketStates)
        {
            var supermasterIds = await agentRepos.FindQueryBy(x => x.RoleId == loginAgent.RoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var queryTicketCompleted = casinoTicketBetDetailRepos.FindQueryBy(c => ticketStates.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoAgentWinlossSummaries = await agentRepos.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetailRepos.FindQueryBy(y => queryTicketCompleted.Contains(y.GameRoundId) && !y.IsCancel && y.CreatedAt >= from.Date && y.CreatedAt <= to.AddDays(1).AddTicks(-1)).Include(c=>c.CasinoTicket),
                                   x => x.AgentId, y => y.CasinoTicket.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       agent.RoleId,
                                       ticket.GameRoundId,
                                       ticket.WinOrLossAmount,
                                       ticket.BetAmount,
                                       ticket.AgentWinLoss,
                                       ticket.MasterWinLoss,
                                       ticket.SupermasterWinLoss,
                                       ticket.CompanyWinLoss,
                                       ticket.Status,
                                       ticket.CasinoTicket.Type
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username,
                                       x.RoleId,
                                       x.GameRoundId
                                   })
                                   .Select(x => new CasinoAgentWinlossModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       RoleId = x.Key.RoleId,
                                       BetCount = x.LongCount(),
                                       Payout = x.Sum(s => (s.Status == CasinoBetStatus.Settled && s.Type != CasinoTransferType.Manual_Settle) ? 0m : s.BetAmount),
                                       WinLose = x.Sum(s => s.WinOrLossAmount ?? 0m),
                                       AgentWinlose = loginAgent.RoleId < Role.Agent.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.AgentWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       MasterWinlose = loginAgent.RoleId < Role.Master.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.MasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       SupermasterWinlose = loginAgent.RoleId < Role.Supermaster.ToInt()
                                                       ? new CasinoWinlossInfoModel
                                                       {
                                                           WinLose = x.Sum(s => s.SupermasterWinLoss),
                                                           Commission = 0m
                                                       }
                                                       : null,
                                       Company = x.Sum(s => s.CompanyWinLoss)
                                   })
                                   .OrderBy(x => x.Username)
                                   .ToListAsync();
            return new GetCasinoAgentWinLossSummaryResultModel
            {
                CasinoAgentWinlossSummaries = casinoAgentWinlossSummaries,
                TotalCasinoAgentWinLoseInfo = new List<TotalCasinoAgentWinLoseInfoModel>
                {
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.AgentWinlose != null).Sum(x => x.AgentWinlose.Subtotal),
                        RoleId = Role.Agent.ToInt()
                    },
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.MasterWinlose != null).Sum(x => x.MasterWinlose.Subtotal),
                        RoleId = Role.Master.ToInt()
                    },
                    new() {
                        TotalWinLose = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.WinLose),
                        TotalCommission = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Commission),
                        TotalSubTotal = casinoAgentWinlossSummaries.Where(x => x.SupermasterWinlose != null).Sum(x => x.SupermasterWinlose.Subtotal),
                        RoleId = Role.Supermaster.ToInt()
                    }
                },
                TotalBetCount = casinoAgentWinlossSummaries.Sum(x => x.BetCount),
                TotalPayout = casinoAgentWinlossSummaries.Sum(x => x.Payout),
                TotalWinLose = casinoAgentWinlossSummaries.Sum(x => x.WinLose),
                TotalCompany = casinoAgentWinlossSummaries.Sum(x => x.Company)
            };
        }
    }
}
