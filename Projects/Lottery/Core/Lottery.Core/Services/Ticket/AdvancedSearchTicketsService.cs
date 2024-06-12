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

public class AdvancedSearchTicketsService : LotteryBaseService<AdvancedSearchTicketsService>, IAdvancedSearchTicketsService
{
    private readonly INormalizeTicketService _normalizeTicketService;

    public AdvancedSearchTicketsService(ILogger<AdvancedSearchTicketsService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        INormalizeTicketService normalizeTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _normalizeTicketService = normalizeTicketService;
    }

    public async Task RefundRejectTickets(List<long> ticketIds)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var tickets = await ticketRepository.FindQueryBy(f => ticketIds.Contains(f.TicketId) || (f.ParentId.HasValue && ticketIds.Contains(f.ParentId.Value))).ToListAsync();
        var parentTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
        foreach (var ticket in parentTickets)
        {
            ticket.State = ticket.IsLive ? TicketState.Reject.ToInt() : TicketState.Refund.ToInt();
            ticket.PlayerWinLoss = 0m;
            ticket.AgentWinLoss = 0m;
            ticket.AgentCommission = 0m;

            ticket.MasterWinLoss = 0m;
            ticket.MasterCommission = 0m;

            ticket.SupermasterWinLoss = 0m;
            ticket.SupermasterCommission = 0m;

            ticket.CompanyWinLoss = 0m;

            ticketRepository.Update(ticket);

            var childs = tickets.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).ToList();
            childs.ForEach(f =>
            {
                f.State = ticket.State;
                f.PlayerWinLoss = ticket.PlayerWinLoss;

                f.AgentWinLoss = ticket.AgentWinLoss;
                f.AgentCommission = ticket.AgentCommission;

                f.MasterWinLoss = ticket.MasterWinLoss;
                f.MasterCommission = ticket.MasterCommission;

                f.SupermasterWinLoss = ticket.SupermasterWinLoss;
                f.SupermasterCommission = ticket.SupermasterCommission;

                f.CompanyWinLoss = ticket.CompanyWinLoss;

                ticketRepository.Update(f);
            });
        }
        await LotteryUow.SaveChangesAsync();
    }

    public async Task<AdvancedSearchTicketsResultModel> Search(AdvancedSearchTicketsModel model)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var ticketQuery = ticketRepository.FindQueryBy(f => !f.ParentId.HasValue);
        if (model.MatchId > 0L) ticketQuery = ticketQuery.Where(f => f.MatchId == model.MatchId);
        if (model.TicketIds.Count > 0) ticketQuery = ticketQuery.Where(f => model.TicketIds.Contains(f.TicketId));
        if (model.Username.Count > 0)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var playerIds = await playerRepository.FindQueryBy(f => model.Username.Any(f1 => f.Username.Contains(f1.ToUpper()))).Select(f => f.PlayerId).ToListAsync();
            if (playerIds.Count > 0) ticketQuery = ticketQuery.Where(f => playerIds.Contains(f.PlayerId));
        }
        if (model.BetKindIds.Count > 0) ticketQuery = ticketQuery.Where(f => model.BetKindIds.Contains(f.BetKindId));
        if (model.RegionId > 0) ticketQuery = ticketQuery.Where(f => f.RegionId == model.RegionId);
        if (model.ChannelId > 0) ticketQuery = ticketQuery.Where(f => f.ChannelId == model.ChannelId);
        if (model.ChooseNumbers.Count > 0)
        {
            var chooseNumbers = new List<string>();
            model.ChooseNumbers.ForEach(f =>
            {
                chooseNumbers.Add(f.NormalizeNumber());
            });
            ticketQuery = ticketQuery.Where(f => chooseNumbers.Contains(f.ChoosenNumbers));
        }
        if (model.States.Count > 0) ticketQuery = ticketQuery.Where(f => model.States.Contains(f.State));
        if (model.Prizes.Count > 0) ticketQuery = ticketQuery.Where(f => model.Prizes.Contains(f.Prize.Value));

        ticketQuery = ticketQuery.OrderByDescending(f => f.CreatedAt);
        var result = await ticketRepository.PagingByAsync(ticketQuery, model.PageIndex, model.PageSize);
        var tickets = result.Items.Select(f => f.ToTicketDetailModel()).ToList();
        _normalizeTicketService.NormalizeTicket(tickets);
        return new AdvancedSearchTicketsResultModel
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