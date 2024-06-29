using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Tickets;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Winlose;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class PlayerTicketService : LotteryBaseService<PlayerTicketService>, IPlayerTicketService
{
    private readonly INormalizeTicketService _normalizeTicketService;

    public PlayerTicketService(ILogger<PlayerTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        INormalizeTicketService normalizeTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _normalizeTicketService = normalizeTicketService;
    }

    public async Task<List<TicketDetailModel>> GetDetailTicket(long ticketId, bool fromPlayer = true)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var currentTicket = await ticketRepository.FindByIdAsync(ticketId);
        if (currentTicket == null) return new List<TicketDetailModel>();
        var data = (await ticketRepository.FindQueryBy(f => (!fromPlayer || (fromPlayer && f.PlayerId == ClientContext.Player.PlayerId)) && f.ParentId.HasValue && f.ParentId.Value == ticketId).ToListAsync())
            .Select(f => f.ToTicketDetailModel())
            .OrderBy(f => f.ChoosenNumbers).ToList();
        return data;
    }

    public async Task<List<TicketDetailModel>> GetPlayerOuts(long playerId)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var outsState = CommonHelper.OutsTicketState();
        var queryData = ticketRepository.FindQueryBy(f => f.PlayerId == playerId && !f.ParentId.HasValue && outsState.Contains(f.State));
        if (ClientContext.Agent.RoleId == Role.Supermaster.ToInt())
        {
            var supermasterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
            queryData = queryData.Where(f => f.SupermasterId == supermasterId);
        }
        else if (ClientContext.Agent.RoleId == Role.Master.ToInt())
        {
            var supermasterId = ClientContext.Agent.SupermasterId;
            var masterId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
            queryData = queryData.Where(f => f.SupermasterId == supermasterId && f.MasterId == masterId);
        }
        else if (ClientContext.Agent.RoleId == Role.Agent.ToInt())
        {
            var supermasterId = ClientContext.Agent.SupermasterId;
            var masterId = ClientContext.Agent.MasterId;
            var agentId = ClientContext.Agent.ParentId == 0L ? ClientContext.Agent.AgentId : ClientContext.Agent.ParentId;
            queryData = queryData.Where(f => f.SupermasterId == supermasterId && f.MasterId == masterId && f.AgentId == agentId);
        }
        var data = await queryData
            .OrderByDescending(f => f.TicketId)
            .Select(f => f.ToTicketDetailModel()).ToListAsync();
        _normalizeTicketService.NormalizeTicket(data);
        return data;
    }

    public async Task<List<TicketDetailModel>> GetPlayerWinloseDetail(WinloseDetailQueryModel model)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var completedState = model.SelectedDraft ? CommonHelper.AllTicketState() : CommonHelper.CompletedTicketState();
        var data = await ticketRepository.FindQueryBy(f => f.PlayerId == model.PlayerId && !f.ParentId.HasValue && f.KickOffTime.Date >= model.FromDate.Date && f.KickOffTime.Date <= model.ToDate.Date && completedState.Contains(f.State))
            .OrderByDescending(f => f.TicketId)
            .Select(f => f.ToTicketDetailModel()).ToListAsync();
        _normalizeTicketService.NormalizeTicket(data);
        return data;
    }

    public async Task<List<TicketDetailModel>> GetRefundRejectTickets()
    {
        var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
        var latestMatch = await matchRepository.GetLatestMatch();
        if (latestMatch == null) return new List<TicketDetailModel>();
        return await InternalGetTicketsByMatch(latestMatch.MatchId, CommonHelper.RefundRejectTicketState());
    }

    public async Task<List<TicketDetailModel>> GetTicketsAsBetList()
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var runningState = CommonHelper.OutsTicketState();
        var data = await ticketRepository.FindQueryBy(f => f.PlayerId == ClientContext.Player.PlayerId && !f.ParentId.HasValue && runningState.Contains(f.State))
            .OrderByDescending(f => f.TicketId)
            .Select(f => f.ToTicketDetailModel()).ToListAsync();
        _normalizeTicketService.NormalizeTicket(data);
        return data;
    }

    public async Task<List<TicketDetailModel>> GetTicketsByMatch(long matchId)
    {
        return await InternalGetTicketsByMatch(matchId);
    }

    private async Task<List<TicketDetailModel>> InternalGetTicketsByMatch(long matchId, List<int> listState = null)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var data = await ticketRepository.FindQueryBy(f => f.PlayerId == ClientContext.Player.PlayerId && !f.ParentId.HasValue && f.MatchId == matchId && (listState == null || (listState != null && listState.Contains(f.State))))
            .OrderByDescending(f => f.TicketId)
            .Select(f => f.ToTicketDetailModel()).ToListAsync();
        _normalizeTicketService.NormalizeTicket(data);
        return data;
    }
}