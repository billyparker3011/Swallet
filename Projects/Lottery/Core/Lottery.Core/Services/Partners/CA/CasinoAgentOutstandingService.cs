using HnMicro.Framework.Enums;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities;
using Lottery.Data.Entities.Partners.Casino;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using static Lottery.Core.Helpers.PartnerHelper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Lottery.Core.Services.Partners.CA
{
    public class CasinoAgentOutstandingService : LotteryBaseService<CasinoAgentOutstandingService>, ICasinoAgentOutstandingService
    {
        public CasinoAgentOutstandingService(ILogger<CasinoAgentOutstandingService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<CasinoAgentOutstandingResultModel> GetCasinoAgentOutstanding(GetCasinoAgentOutstandingModel model)
        {
            var agentRepository = LotteryUow.GetRepository<IAgentRepository>();
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var casinoTicketRepository = LotteryUow.GetRepository<ICasinoTicketRepository>();
            var casinoTicketBetDetailRepository = LotteryUow.GetRepository<ICasinoTicketBetDetailRepository>();
            var listCasinoAgentOustandings = new List<CasinoAgentOutstandingModel>();
            var targetAgentId = model.AgentId.HasValue ? model.AgentId.Value
                                                : ClientContext.Agent.ParentId == 0L
                                                    ? ClientContext.Agent.AgentId
                                                    : ClientContext.Agent.ParentId;
            var targetRoleId = model.RoleId.HasValue ? model.RoleId.Value : ClientContext.Agent.RoleId;
            if (targetRoleId < ClientContext.Agent.RoleId) return new CasinoAgentOutstandingResultModel();
            var outsState = PartnerHelper.CasinoBetStatus.BetRunning.ToList();
            switch (targetRoleId)
            {
                case (int)Role.Company:
                    listCasinoAgentOustandings = await GetCasinoAgentOutstandingsOfCompany(agentRepository, casinoTicketRepository, casinoTicketBetDetailRepository, targetAgentId, targetRoleId, model);
                    break;
                case (int)Role.Supermaster:
                    listCasinoAgentOustandings = await GetCasinoAgentOutstandingsOfSupermaster(agentRepository, casinoTicketRepository, casinoTicketBetDetailRepository, targetAgentId, targetRoleId, model);
                    break;
                case (int)Role.Master:
                    listCasinoAgentOustandings = await GetCasinoAgentOutstandingsOfMaster(agentRepository, casinoTicketRepository, casinoTicketBetDetailRepository, targetAgentId, targetRoleId, model);
                    break;
                case (int)Role.Agent:
                    listCasinoAgentOustandings = await GetCasinoAgentOutstandingsOfAgent(playerRepository, casinoTicketRepository, casinoTicketBetDetailRepository, targetAgentId, targetRoleId, model);
                    break;
                default:
                    break;
            }

            return new CasinoAgentOutstandingResultModel
            {
                CasinoAgentOuts = listCasinoAgentOustandings,
                SummaryBetCount = listCasinoAgentOustandings.Sum(x => x.TotalBetCount),
                SummaryPayout = listCasinoAgentOustandings.Sum(x => x.TotalPayout)
            };
        }

        private async Task<List<CasinoAgentOutstandingModel>> GetCasinoAgentOutstandingsOfAgent(IPlayerRepository playerRepository, ICasinoTicketRepository casinoTicketRepository, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepository, long targetAgentId, int targetRoleId, GetCasinoAgentOutstandingModel model)
        {

            var playerIds = await playerRepository.FindQueryBy(x => x.AgentId == targetAgentId).Select(x => x.PlayerId).ToListAsync();
            var query = casinoTicketBetDetailRepository.FindQueryBy(c => playerIds.Contains(c.CasinoTicket.PlayerId) && !c.IsCancel).Include(c => c.CasinoTicket).ThenInclude(c => c.Player); 
            var queryTicketCompleted = query.Where(c => PartnerHelper.CasinoBetStatus.BetCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetail = query.Where(c => PartnerHelper.CasinoBetStatus.BetRunning.Contains(c.Status) && !queryTicketCompleted.Contains(c.GameRoundId));

            var casinoAgentOutsQuery = casinoTicketBetDetail.Select(c => new
                                   {
                                       c.CasinoTicket.PlayerId,
                                       c.CasinoTicket.Player.Username,
                                       c.GameRoundId,
                                       Payout = c.BetAmount,
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.PlayerId,
                                       x.Username
                                   })
                                   .Select(x => new CasinoAgentOutstandingModel
                                   {
                                       AgentId = x.Key.PlayerId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.Select(c => c.GameRoundId).Distinct().LongCount(),
                                       TotalPayout = x.Sum(s => s.Payout)
                                   }).AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                casinoAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                casinoAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await casinoAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CasinoAgentOutstandingModel>> GetCasinoAgentOutstandingsOfMaster(IAgentRepository agentRepository, ICasinoTicketRepository casinoTicketRepository, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepository, long targetAgentId, int targetRoleId, GetCasinoAgentOutstandingModel model)
        {
            var agentIds = await agentRepository.FindQueryBy(x => x.MasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var query = casinoTicketBetDetailRepository.FindQueryBy(c => agentIds.Contains(c.CasinoTicket.AgentId) && !c.IsCancel).Include(c => c.CasinoTicket);             
            var queryTicketCompleted = query.Where(c => PartnerHelper.CasinoBetStatus.BetCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetail = query.Where(c => PartnerHelper.CasinoBetStatus.BetRunning.Contains(c.Status) && !queryTicketCompleted.Contains(c.GameRoundId));

            var casinoAgentOutsQuery = agentRepository.FindQueryBy(x => agentIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetail, x => x.AgentId, y => y.CasinoTicket.AgentId, (agent, ticket) => new
                                    {
                                        agent.AgentId,
                                        agent.Username,
                                        ticket.GameRoundId,
                                        Payout = ticket.BetAmount,
                                    })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username
                                   })
                                   .Select(x => new CasinoAgentOutstandingModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.Select(c => c.GameRoundId).Distinct().LongCount(),
                                       TotalPayout = x.Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await casinoAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CasinoAgentOutstandingModel>> GetCasinoAgentOutstandingsOfSupermaster(IAgentRepository agentRepository, ICasinoTicketRepository casinoTicketRepository, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepository, long targetAgentId, int targetRoleId, GetCasinoAgentOutstandingModel model)
        {
            var masterIds = await agentRepository.FindQueryBy(x => x.SupermasterId == targetAgentId && x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var query = casinoTicketBetDetailRepository.FindQueryBy(c => masterIds.Contains(c.CasinoTicket.AgentId) && !c.IsCancel).Include(c => c.CasinoTicket);
            var queryTicketCompleted = query.Where(c => PartnerHelper.CasinoBetStatus.BetCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetail = query.Where(c => PartnerHelper.CasinoBetStatus.BetRunning.Contains(c.Status) && !queryTicketCompleted.Contains(c.GameRoundId));

            var casinoAgentOutsQuery = agentRepository.FindQueryBy(x => masterIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetail, x => x.AgentId, y => y.CasinoTicket.MasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       ticket.GameRoundId,
                                       Payout = ticket.BetAmount,
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username
                                   })
                                   .Select(x => new CasinoAgentOutstandingModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.Select(c => c.GameRoundId).Distinct().LongCount(),
                                       TotalPayout = x.Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await casinoAgentOutsQuery.ToListAsync();
        }

        private async Task<List<CasinoAgentOutstandingModel>> GetCasinoAgentOutstandingsOfCompany(IAgentRepository agentRepository, ICasinoTicketRepository casinoTicketRepository, ICasinoTicketBetDetailRepository casinoTicketBetDetailRepository, long targetAgentId, int targetRoleId, GetCasinoAgentOutstandingModel model)
        {
            var supermasterIds = await agentRepository.FindQueryBy(x => x.RoleId == targetRoleId + 1 && x.ParentId == 0L).Select(x => x.AgentId).ToListAsync();
            var query = casinoTicketBetDetailRepository.FindQueryBy(c => supermasterIds.Contains(c.CasinoTicket.AgentId) && !c.IsCancel).Include(c => c.CasinoTicket);
            var queryTicketCompleted = query.Where(c => PartnerHelper.CasinoBetStatus.BetCompleted.Contains(c.Status)).Select(c => c.GameRoundId).Distinct();
            var casinoTicketBetDetail = query.Where(c => PartnerHelper.CasinoBetStatus.BetRunning.Contains(c.Status) && !queryTicketCompleted.Contains(c.GameRoundId));

            var casinoAgentOutsQuery = agentRepository.FindQueryBy(x => supermasterIds.Contains(x.AgentId))
                                   .Join(casinoTicketBetDetail, x => x.AgentId, y => y.CasinoTicket.SupermasterId, (agent, ticket) => new
                                   {
                                       agent.AgentId,
                                       agent.Username,
                                       ticket.GameRoundId,
                                       Payout = ticket.BetAmount,
                                   })
                                   .GroupBy(x => new
                                   {
                                       x.AgentId,
                                       x.Username
                                   })
                                   .Select(x => new CasinoAgentOutstandingModel
                                   {
                                       AgentId = x.Key.AgentId,
                                       Username = x.Key.Username,
                                       AgentRole = Role.Player,
                                       TotalBetCount = x.Select(c => c.GameRoundId).Distinct().LongCount(),
                                       TotalPayout = x.Sum(s => s.Payout)
                                   })
                                   .AsQueryable();

            if (model.SortType == SortType.Descending)
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderByDescending(GetSortCockFightAgentOutsProperty(model));
            }
            else
            {
                casinoAgentOutsQuery = casinoAgentOutsQuery.OrderBy(GetSortCockFightAgentOutsProperty(model));
            }

            return await casinoAgentOutsQuery.ToListAsync();
        }

        private Expression<Func<CasinoAgentOutstandingModel, object>> GetSortCockFightAgentOutsProperty(GetCasinoAgentOutstandingModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return casinoAgentOuts => casinoAgentOuts.Username;
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
