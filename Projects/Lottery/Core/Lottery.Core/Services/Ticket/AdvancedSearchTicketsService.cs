using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Helpers.Converters.Tickets;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class AdvancedSearchTicketsService : LotteryBaseService<AdvancedSearchTicketsService>, IAdvancedSearchTicketsService
{
    private readonly ITicketProcessor _ticketProcessor;
    private readonly IProcessTicketService _processTicketService;
    private readonly INormalizeTicketService _normalizeTicketService;

    public AdvancedSearchTicketsService(ILogger<AdvancedSearchTicketsService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        ITicketProcessor ticketProcessor,
        IProcessTicketService processTicketService,
        INormalizeTicketService normalizeTicketService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _ticketProcessor = ticketProcessor;
        _processTicketService = processTicketService;
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
            ticket.DraftPlayerWinLoss = 0m;

            ticket.AgentWinLoss = 0m;
            ticket.AgentCommission = 0m;
            ticket.DraftAgentWinLoss = 0m;
            ticket.DraftAgentCommission = 0m;

            ticket.MasterWinLoss = 0m;
            ticket.MasterCommission = 0m;
            ticket.DraftMasterWinLoss = 0m;
            ticket.DraftMasterCommission = 0m;

            ticket.SupermasterWinLoss = 0m;
            ticket.SupermasterCommission = 0m;
            ticket.DraftSupermasterWinLoss = 0m;
            ticket.DraftSupermasterCommission = 0m;

            ticket.CompanyWinLoss = 0m;
            ticket.DraftCompanyWinLoss = 0m;

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

        //  Process outs by player
        //  Process outs by player, number
    }

    public async Task RefundRejectTicketsByNumbers(List<long> ticketIds, List<int> numbers)
    {
        var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
        var playerOuts = new Dictionary<long, Dictionary<long, decimal>>();

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var tickets = await ticketRepository.FindQueryBy(f => ticketIds.Contains(f.TicketId) || (f.ParentId.HasValue && ticketIds.Contains(f.ParentId.Value))).ToListAsync();
        var parentTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
        foreach (var ticket in parentTickets)
        {
            var childs = tickets.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).ToList();

            var result = _ticketProcessor.AllowRefundRejectTicketsByNumbers(ticket.BetKindId, new RefundRejectTicketModel
            {
                RefundRejectNumbers = numbers.Select(f => f.NormalizeNumber()).ToList(),
                Ticket = new RefundRejectTicketDetailModel
                {
                    TicketId = ticket.TicketId,
                    ChoosenNumbers = ticket.ChoosenNumbers,
                    State = ticket.State.ToEnum<TicketState>(),
                    IsLive = ticket.IsLive,
                    Stake = ticket.Stake,
                    PlayerPayout = ticket.PlayerPayout,
                    AgentPayout = ticket.AgentPayout,
                    MasterPayout = ticket.MasterPayout,
                    SupermasterPayout = ticket.SupermasterPayout,
                    CompanyPayout = ticket.CompanyPayout
                },
                Children = childs.Select(f => new RefundRejectTicketDetailModel
                {
                    TicketId = f.TicketId,
                    ChoosenNumbers = f.ChoosenNumbers,
                    State = f.State.ToEnum<TicketState>(),
                    IsLive = ticket.IsLive,
                    Stake = f.Stake,
                    PlayerPayout = f.PlayerPayout,
                    AgentPayout = f.AgentPayout,
                    MasterPayout = f.MasterPayout,
                    SupermasterPayout = f.SupermasterPayout,
                    CompanyPayout = f.CompanyPayout
                }).ToList()
            });
            if (result == null || !result.Allow) continue;
            var oldPlayerPayout = ticket.PlayerPayout;

            ticket.State = result.Ticket.State.ToInt();
            if (refundRejectTicketState.Contains(ticket.State))
            {
                ticket.PlayerWinLoss = 0m;
                ticket.DraftPlayerWinLoss = 0m;

                ticket.AgentWinLoss = 0m;
                ticket.AgentCommission = 0m;
                ticket.DraftAgentWinLoss = 0m;
                ticket.DraftAgentCommission = 0m;

                ticket.MasterWinLoss = 0m;
                ticket.MasterCommission = 0m;
                ticket.DraftMasterWinLoss = 0m;
                ticket.DraftMasterCommission = 0m;

                ticket.SupermasterWinLoss = 0m;
                ticket.SupermasterCommission = 0m;
                ticket.DraftSupermasterWinLoss = 0m;
                ticket.DraftSupermasterCommission = 0m;

                ticket.CompanyWinLoss = 0m;
                ticket.DraftCompanyWinLoss = 0m;
            }
            else
            {
                ticket.Stake = result.Ticket.Stake;
                ticket.PlayerPayout = result.Ticket.PlayerPayout;
                ticket.AgentPayout = result.Ticket.PlayerPayout;
                ticket.MasterPayout = result.Ticket.PlayerPayout;
                ticket.SupermasterPayout = result.Ticket.PlayerPayout;
                ticket.CompanyPayout = result.Ticket.PlayerPayout;
            }
            ticketRepository.Update(ticket);
            childs.ForEach(f =>
            {
                var updatedChild = result.Children.FirstOrDefault(f1 => f1.TicketId == f.TicketId);
                if (updatedChild == null) return;

                f.State = updatedChild.State.ToInt();
                if (refundRejectTicketState.Contains(f.State))
                {
                    f.PlayerWinLoss = 0m;
                    f.DraftPlayerWinLoss = 0m;

                    f.AgentWinLoss = 0m;
                    f.AgentCommission = 0m;
                    f.DraftAgentWinLoss = 0m;
                    f.DraftAgentCommission = 0m;

                    f.MasterWinLoss = 0m;
                    f.MasterCommission = 0m;
                    f.DraftMasterWinLoss = 0m;
                    f.DraftMasterCommission = 0m;

                    f.SupermasterWinLoss = 0m;
                    f.SupermasterCommission = 0m;
                    f.DraftSupermasterWinLoss = 0m;
                    f.DraftSupermasterCommission = 0m;

                    f.CompanyWinLoss = 0m;
                    f.DraftCompanyWinLoss = 0m;
                }
                ticketRepository.Update(f);
            });

            var newPlayerPayout = ticket.PlayerPayout;
            var subPlayerPayout = Math.Abs(newPlayerPayout - oldPlayerPayout);
            if (!playerOuts.TryGetValue(ticket.PlayerId, out Dictionary<long, decimal> playerOutsDetail))
            {
                playerOutsDetail = new Dictionary<long, decimal>();
                playerOuts[ticket.PlayerId] = playerOutsDetail;
            }
            if (playerOutsDetail.ContainsKey(ticket.MatchId)) playerOutsDetail[ticket.MatchId] += subPlayerPayout;
            else playerOutsDetail[ticket.MatchId] = subPlayerPayout;
        }
        await LotteryUow.SaveChangesAsync();

        await _processTicketService.UpdateOutsByMatchCache(playerOuts);
        //await _processTicketService.BuildOutsByMatchAndNumbersCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.PointsByMatchAndNumbers, pointByNumbers);
        //if (enableStats) await _processTicketService.BuildStatsByMatchBetKindAndNumbers(processValidation.Match.MatchId, processValidation.BetKind.Id, pointByNumbers, payoutByNumbers);
    }

    public async Task<AdvancedSearchTicketsResultModel> Search(AdvancedSearchTicketsModel model)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
        var ticketQuery = ticketRepository.FindQueryBy(f => !f.ParentId.HasValue);
        if (model.MatchId > 0L) ticketQuery = ticketQuery.Where(f => f.MatchId == model.MatchId);
        if (model.TicketIds.Count > 0) ticketQuery = ticketQuery.Where(f => model.TicketIds.Contains(f.TicketId));
        if (model.Username.Count > 0)
        {
            var playerIds = await playerRepository.FindQueryBy(f => model.Username.Any(f1 => f.Username.Contains(f1.ToUpper()))).Select(f => f.PlayerId).ToListAsync();
            if (playerIds.Count > 0) ticketQuery = ticketQuery.Where(f => playerIds.Contains(f.PlayerId));
        }
        if (model.BetKindIds.Count > 0) ticketQuery = ticketQuery.Where(f => model.BetKindIds.Contains(f.BetKindId));
        if (model.RegionId > 0) ticketQuery = ticketQuery.Where(f => f.RegionId == model.RegionId);
        if (model.ChannelId > 0) ticketQuery = ticketQuery.Where(f => f.ChannelId == model.ChannelId);
        if (model.ChooseNumbers.Count > 0) ticketQuery = ticketQuery.Where(model.ChooseNumbers.ContainsNumbers(model.ContainNumberOperator.ToEnum<Core.Enums.ContainNumberOperator>()));
        if (model.States.Count > 0) ticketQuery = ticketQuery.Where(f => model.States.Contains(f.State));
        if (model.Prizes.Count > 0) ticketQuery = ticketQuery.Where(f => model.Prizes.Contains(f.Prize.Value));

        ticketQuery = ticketQuery.OrderByDescending(f => f.TicketId);
        var result = await ticketRepository.PagingByAsync(ticketQuery, model.PageIndex, model.PageSize);
        var tickets = result.Items.Select(f => f.ToTicketDetailModel()).ToList();
        _normalizeTicketService.NormalizeTicket(tickets);

        var listPlayerId = tickets.Select(f => f.PlayerId).Distinct().ToList();
        var players = await playerRepository.FindQueryBy(f => listPlayerId.Contains(f.PlayerId)).ToListAsync();
        _normalizeTicketService.NormalizePlayer(tickets, players.ToDictionary(f => f.PlayerId, f => f.Username));

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