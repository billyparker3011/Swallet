using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Tickets;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class BroadCasterTicketService : LotteryBaseService<BroadCasterTicketService>, IBroadCasterTicketService
{
    private readonly INormalizeTicketService _normalizeTicketService;

    public BroadCasterTicketService(ILogger<BroadCasterTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        INormalizeTicketService normalizeTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _normalizeTicketService = normalizeTicketService;
    }

    public async Task<List<TicketDetailModel>> GetBroadCasterOuts(long betkindId)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var outsState = CommonHelper.OutsTicketState();
        var queryData = ticketRepository.FindQueryBy(f => f.BetKindId == betkindId && !f.ParentId.HasValue && outsState.Contains(f.State));
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

        var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
        var listPlayerId = data.Select(f => f.PlayerId).Distinct().ToList();
        var players = await playerRepository.FindQueryBy(f => listPlayerId.Contains(f.PlayerId)).ToListAsync();
        _normalizeTicketService.NormalizePlayer(data, players.ToDictionary(f => f.PlayerId, f => f.Username));
        return data;
    }
}