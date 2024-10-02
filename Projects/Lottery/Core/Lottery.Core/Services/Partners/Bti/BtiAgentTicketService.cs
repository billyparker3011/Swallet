using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Bti;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Lottery.Core.Partners.Helpers.BtiHelper;

namespace Lottery.Core.Services.Partners.Bti
{
    public class BtiAgentTicketService : LotteryBaseService<BtiAgentTicketService>, IBtiAgentTicketService
    {
        public BtiAgentTicketService(ILogger<BtiAgentTicketService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ILotteryClientContext clientContext,
            ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<BtiAgentTicketModel> GetBtiRefundRejectTickets(AgentTicketModel model)
        {
            var BtiTicketRepository = LotteryUow.GetRepository<IBtiTicketRepository>();
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();

            var targetAgentId = ClientContext.Agent.ParentId != 0L ? ClientContext.Agent.ParentId : ClientContext.Agent.AgentId;
            var clientAgent = await agentRepos.FindByIdAsync(targetAgentId) ?? throw new NotFoundException();
            var lastestMatchDayCodeTicket = await GetLastestTicketByAgentRole(BtiTicketRepository, clientAgent);
            if (lastestMatchDayCodeTicket == null) return new BtiAgentTicketModel();
            var rejectRefundState = new List<int>() { BtiTicketStatusHelper.Cancel };
            return await InternalGetBtiTickets(rejectRefundState, model);
        } 

        public async Task<BtiAgentTicketModel> BtiLatestTickets(AgentTicketModel model)
        {
            var runningState = BtiTicketStatusHelper.Betting;
            return await InternalGetBtiTickets(runningState, model);
        }

        private async Task<BtiTicket> GetLastestTicketByAgentRole(IBtiTicketRepository BtiTicketRepository, Data.Entities.Agent clientAgent)
        {
            switch (clientAgent.RoleId)
            {
                case (int)Role.Company:
                    return await BtiTicketRepository.FindQueryBy(x => !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Supermaster:
                    return await BtiTicketRepository.FindQueryBy(x => x.SupermasterId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Master:
                    return await BtiTicketRepository.FindQueryBy(x => x.MasterId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                case (int)Role.Agent:
                    return await BtiTicketRepository.FindQueryBy(x => x.AgentId == clientAgent.AgentId && !x.ParentId.HasValue).OrderByDescending(x => x.TicketCreatedDate).FirstOrDefaultAsync();
                default:
                    return null;
            }
        }

        private async Task<BtiAgentTicketModel> InternalGetBtiTickets(List<int> stateIds, AgentTicketModel model)
        {
            var currentRoleId = ClientContext.Agent.RoleId;
            var currentAgentId = ClientContext.Agent.ParentId == 0 ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;

            var BtiTicketRepository = LotteryUow.GetRepository<IBtiTicketRepository>();
            var BtiAgentTicketQuery = BtiTicketRepository.FindQueryBy(f => !f.ParentId.HasValue && stateIds.Contains(f.Status));         

            if (currentRoleId == Role.Company.ToInt()) BtiAgentTicketQuery = BtiAgentTicketQuery.Where(f => 1 == 1);
            else if (currentRoleId == Role.Supermaster.ToInt()) BtiAgentTicketQuery = BtiAgentTicketQuery.Where(f => f.SupermasterId == currentAgentId);
            else if (currentRoleId == Role.Master.ToInt()) BtiAgentTicketQuery = BtiAgentTicketQuery.Where(f => f.MasterId == currentAgentId);
            else BtiAgentTicketQuery = BtiAgentTicketQuery.Where(f => f.AgentId == currentAgentId);

            BtiAgentTicketQuery = BtiAgentTicketQuery.OrderByDescending(f => f.CreatedAt);

            var result = await BtiTicketRepository.PagingByAsync(BtiAgentTicketQuery, model.PageIndex, model.PageSize);
            var tickets = result.Items.Select(f => new BtiTicketModel
            {
                //PlayerId = f.PlayerId,
                //TicketId = f.Sid,
                //BetKindId = f.BetKindId,
                //FightNumber = f.FightNumber,
                //Odds = f.Odds ?? 0m,
                //BetAmount = f.BetAmount ?? 0m,
                //MatchDayCode = f.MatchDayCode,
                ////Result = ,
                ////Selection = ,
                ////Status = ,
                //TicketAmount = f.TicketAmount ?? 0m,
                //WinlossAmount = f.WinlossAmount ?? 0m,
                //IpAddress = f.IpAddress,
                ////UserAgent = !string.IsNullOrEmpty(f.UserAgent) ? f.UserAgent.GetPlatform() : f.UserAgent,
                //ArenaCode = f.ArenaCode,
                //CurrencyCode = f.CurrencyCode,
                //DateCreated = f.TicketCreatedDate,
                //OddsType = f.OddsType
            }).ToList();

            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var listPlayerId = tickets.Select(f => f.PlayerId).Distinct().ToList();
            var players = await playerRepository.FindQueryBy(f => listPlayerId.Contains(f.PlayerId)).ToListAsync();
            var dictPlayer = players.ToDictionary(f => f.PlayerId, f => f.Username);
            tickets.ForEach(f =>
            {
                if (!dictPlayer.TryGetValue(f.PlayerId, out string username)) return;
                f.Username = username;
            });
            return new BtiAgentTicketModel
            {
                Items = tickets,
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }
    }
}
