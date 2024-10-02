using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Tickets;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class AgentTicketService : LotteryBaseService<AgentTicketService>, IAgentTicketService
{
    private readonly INormalizeTicketService _normalizeTicketService;

    public AgentTicketService(ILogger<AgentTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        INormalizeTicketService normalizeTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _normalizeTicketService = normalizeTicketService;
    }

    public async Task<AgentTicketResultModel> GetRefundRejectTickets(AgentTicketModel model)
    {
        var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
        var latestMatch = await matchRepository.GetLatestMatch();
        if (latestMatch == null) return new AgentTicketResultModel();
        var rejectRefundState = CommonHelper.RefundRejectTicketState();
        return await InternalGetTickets(rejectRefundState, model, latestMatch.MatchId);
    }

    public async Task<AgentTicketResultModel> LatestTickets(AgentTicketModel model)
    {
        var runningState = CommonHelper.OutsTicketState();
        return await InternalGetTickets(runningState, model);
    }

    private async Task<AgentTicketResultModel> InternalGetTickets(List<int> stateIds, AgentTicketModel model, long matchId = 0L)
    {
        var currentRoleId = ClientContext.Agent.RoleId;
        var currentAgentId = ClientContext.Agent.ParentId == 0 ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var ticketQuery = ticketRepository.FindQueryBy(f => !f.ParentId.HasValue && stateIds.Contains(f.State));
        if (matchId > 0L) ticketQuery.Where(f => f.MatchId == matchId);

        if (currentRoleId == Role.Company.ToInt()) ticketQuery = ticketQuery.Where(f => 1 == 1);
        else if (currentRoleId == Role.Supermaster.ToInt()) ticketQuery = ticketQuery.Where(f => f.SupermasterId == currentAgentId);
        else if (currentRoleId == Role.Master.ToInt()) ticketQuery = ticketQuery.Where(f => f.MasterId == currentAgentId);
        else ticketQuery = ticketQuery.Where(f => f.AgentId == currentAgentId);

        ticketQuery = ticketQuery.OrderByDescending(f => f.CreatedAt);

        var result = await ticketRepository.PagingByAsync(ticketQuery, model.PageIndex, model.PageSize);
        var tickets = result.Items.Select(f => f.ToTicketDetailModel()).ToList();
        _normalizeTicketService.NormalizeTicket(tickets);

        var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
        var listPlayerId = tickets.Select(f => f.PlayerId).Distinct().ToList();
        var players = await playerRepository.FindQueryBy(f => listPlayerId.Contains(f.PlayerId)).ToListAsync();
        _normalizeTicketService.NormalizePlayer(tickets, players.ToDictionary(f => f.PlayerId, f => f.Username));
        return new AgentTicketResultModel
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