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
        var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
        var playerOuts = new Dictionary<long, Dictionary<long, decimal>>();
        var playerMatchByNumbersOuts = new Dictionary<long, Dictionary<long, Dictionary<int, decimal>>>(); //  Key = PlayerId; Key = MatchId; Key = Number
        var playerMatchByNumbersPoints = new Dictionary<long, Dictionary<long, Dictionary<int, decimal>>>(); //  Key = PlayerId; Key = MatchId; Key = Number
        var betKindMatchByNumbersOuts = new Dictionary<int, Dictionary<long, Dictionary<int, decimal>>>();  //  Key = BetKindId; Key = MatchId; Key = Number
        var betKindMatchByNumbersPoints = new Dictionary<int, Dictionary<long, Dictionary<int, decimal>>>();  //  Key = BetKindId; Key = MatchId; Key = Number

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var tickets = await ticketRepository.FindQueryBy(f => ticketIds.Contains(f.TicketId) || (f.ParentId.HasValue && ticketIds.Contains(f.ParentId.Value))).ToListAsync();
        var parentTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
        foreach (var ticket in parentTickets)
        {
            if (refundRejectTicketState.Contains(ticket.State)) continue;

            var childs = tickets.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).ToList();
            var result = _ticketProcessor.RefundRejectTicket(ticket.BetKindId, new RefundRejectTicketModel
            {
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
            if (result == null) continue;
            var oldPlayerPayout = ticket.PlayerPayout;

            ticket.State = ticket.IsLive.GetRefundRejectStateByIsLive().ToInt();
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

            //  Process Outs of Player
            InternalProcessOutsOfPlayer(ticket, playerOuts, result.DifferentPlayerPayout);

            //  Process Outs of Player and Match
            InternalProcessOutsOfPlayerByMatch(playerMatchByNumbersOuts, ticket, result.OutsByNumbers);

            //  Process Points of Player and Match
            InternalProcessPointsOfPlayerByMatch(playerMatchByNumbersPoints, ticket, result.PointsByNumbers);

            //  Process Outs of BetKind and Match
            var enableStats = _ticketProcessor.EnableStats(ticket.BetKindId);
            if (!enableStats) continue;
            InternalProcessOutsOfBetKindAndMatch(betKindMatchByNumbersOuts, ticket, result.OutsByBetKind);
            InternalProcessPointsOfBetKindAndMatch(betKindMatchByNumbersPoints, ticket, result.PointsByBetKind);
        }
        await LotteryUow.SaveChangesAsync();

        await _processTicketService.UpdateOutsByMatchCache(playerOuts);
        await _processTicketService.UpdatePointsByMatchAndNumbersCache(playerMatchByNumbersPoints);
        await _processTicketService.UpdateStatsByMatchBetKindAndNumbers(betKindMatchByNumbersOuts, betKindMatchByNumbersPoints);
    }

    public async Task RefundRejectTicketsByNumbers(List<long> ticketIds, List<int> numbers)
    {
        var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
        var playerOuts = new Dictionary<long, Dictionary<long, decimal>>();
        var playerMatchByNumbersOuts = new Dictionary<long, Dictionary<long, Dictionary<int, decimal>>>(); //  Key = PlayerId; Key = MatchId; Key = Number
        var playerMatchByNumbersPoints = new Dictionary<long, Dictionary<long, Dictionary<int, decimal>>>(); //  Key = PlayerId; Key = MatchId; Key = Number
        var betKindMatchByNumbersOuts = new Dictionary<int, Dictionary<long, Dictionary<int, decimal>>>();  //  Key = BetKindId; Key = MatchId; Key = Number
        var betKindMatchByNumbersPoints = new Dictionary<int, Dictionary<long, Dictionary<int, decimal>>>();  //  Key = BetKindId; Key = MatchId; Key = Number

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var tickets = await ticketRepository.FindQueryBy(f => ticketIds.Contains(f.TicketId) || (f.ParentId.HasValue && ticketIds.Contains(f.ParentId.Value))).ToListAsync();
        var parentTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
        foreach (var ticket in parentTickets)
        {
            //if (refundRejectTicketState.Contains(ticket.State)) continue;

            var childs = tickets.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).ToList();

            var result = _ticketProcessor.RefundRejectTicketByNumbers(ticket.BetKindId, new RefundRejectTicketByNumbersModel
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

            //  Process Outs of Player
            InternalProcessOutsOfPlayer(ticket, playerOuts, result.DifferentPlayerPayout);

            //  Process Outs of Player and Match
            InternalProcessOutsOfPlayerByMatch(playerMatchByNumbersOuts, ticket, result.OutsByNumbers);

            //  Process Points of Player and Match
            InternalProcessPointsOfPlayerByMatch(playerMatchByNumbersPoints, ticket, result.PointsByNumbers);

            //  Process Outs of BetKind and Match
            var enableStats = _ticketProcessor.EnableStats(ticket.BetKindId);
            if (!enableStats) continue;
            InternalProcessOutsOfBetKindAndMatch(betKindMatchByNumbersOuts, ticket, result.OutsByBetKind);
            InternalProcessPointsOfBetKindAndMatch(betKindMatchByNumbersPoints, ticket, result.PointsByBetKind);
        }
        await LotteryUow.SaveChangesAsync();

        await _processTicketService.UpdateOutsByMatchCache(playerOuts);
        await _processTicketService.UpdatePointsByMatchAndNumbersCache(playerMatchByNumbersPoints);
        await _processTicketService.UpdateStatsByMatchBetKindAndNumbers(betKindMatchByNumbersOuts, betKindMatchByNumbersPoints);
    }

    private void InternalProcessPointsOfBetKindAndMatch(Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> betKindMatchByNumbersPoints, Data.Entities.Ticket ticket, Dictionary<int, decimal> pointsByBetKind)
    {
        if (!betKindMatchByNumbersPoints.TryGetValue(ticket.BetKindId, out Dictionary<long, Dictionary<int, decimal>> pointsByBetKindAndMatch))
        {
            pointsByBetKindAndMatch = new Dictionary<long, Dictionary<int, decimal>>();
            betKindMatchByNumbersPoints[ticket.BetKindId] = pointsByBetKindAndMatch;
        }
        if (!pointsByBetKindAndMatch.TryGetValue(ticket.MatchId, out Dictionary<int, decimal> pointsByBetKindAndMatchDetail))
        {
            pointsByBetKindAndMatchDetail = new Dictionary<int, decimal>();
            pointsByBetKindAndMatch[ticket.MatchId] = pointsByBetKindAndMatchDetail;
        }
        foreach (var item in pointsByBetKind)
        {
            if (pointsByBetKindAndMatchDetail.ContainsKey(item.Key)) pointsByBetKindAndMatchDetail[item.Key] += item.Value;
            else pointsByBetKindAndMatchDetail[item.Key] = item.Value;
        }
    }

    private void InternalProcessOutsOfBetKindAndMatch(Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> betKindMatchByNumbersOuts, Data.Entities.Ticket ticket, Dictionary<int, decimal> outsByBetKind)
    {
        if (!betKindMatchByNumbersOuts.TryGetValue(ticket.BetKindId, out Dictionary<long, Dictionary<int, decimal>> outsByBetKindAndMatch))
        {
            outsByBetKindAndMatch = new Dictionary<long, Dictionary<int, decimal>>();
            betKindMatchByNumbersOuts[ticket.BetKindId] = outsByBetKindAndMatch;
        }
        if (!outsByBetKindAndMatch.TryGetValue(ticket.MatchId, out Dictionary<int, decimal> outsByBetKindAndMatchDetail))
        {
            outsByBetKindAndMatchDetail = new Dictionary<int, decimal>();
            outsByBetKindAndMatch[ticket.MatchId] = outsByBetKindAndMatchDetail;
        }
        foreach (var item in outsByBetKind)
        {
            if (outsByBetKindAndMatchDetail.ContainsKey(item.Key)) outsByBetKindAndMatchDetail[item.Key] += item.Value;
            else outsByBetKindAndMatchDetail[item.Key] = item.Value;
        }
    }

    private void InternalProcessPointsOfPlayerByMatch(Dictionary<long, Dictionary<long, Dictionary<int, decimal>>> playerMatchByNumbersPoints, Data.Entities.Ticket ticket, Dictionary<int, decimal> pointsByNumbers)
    {
        if (!playerMatchByNumbersPoints.TryGetValue(ticket.PlayerId, out Dictionary<long, Dictionary<int, decimal>> pointsByPlayerAndMatch))
        {
            pointsByPlayerAndMatch = new Dictionary<long, Dictionary<int, decimal>>();
            playerMatchByNumbersPoints[ticket.PlayerId] = pointsByPlayerAndMatch;
        }
        if (!pointsByPlayerAndMatch.TryGetValue(ticket.MatchId, out Dictionary<int, decimal> pointsByPlayerAndMatchDetail))
        {
            pointsByPlayerAndMatchDetail = new Dictionary<int, decimal>();
            pointsByPlayerAndMatch[ticket.MatchId] = pointsByPlayerAndMatchDetail;
        }
        foreach (var item in pointsByNumbers)
        {
            if (pointsByPlayerAndMatchDetail.ContainsKey(item.Key)) pointsByPlayerAndMatchDetail[item.Key] += item.Value;
            else pointsByPlayerAndMatchDetail[item.Key] = item.Value;
        }
    }

    private void InternalProcessOutsOfPlayerByMatch(Dictionary<long, Dictionary<long, Dictionary<int, decimal>>> playerMatchByNumbersOuts, Data.Entities.Ticket ticket, Dictionary<int, decimal> outsByNumbers)
    {
        if (!playerMatchByNumbersOuts.TryGetValue(ticket.PlayerId, out Dictionary<long, Dictionary<int, decimal>> outsByPlayerAndMatch))
        {
            outsByPlayerAndMatch = new Dictionary<long, Dictionary<int, decimal>>();
            playerMatchByNumbersOuts[ticket.PlayerId] = outsByPlayerAndMatch;
        }
        if (!outsByPlayerAndMatch.TryGetValue(ticket.MatchId, out Dictionary<int, decimal> outsByPlayerAndMatchDetail))
        {
            outsByPlayerAndMatchDetail = new Dictionary<int, decimal>();
            outsByPlayerAndMatch[ticket.MatchId] = outsByPlayerAndMatchDetail;
        }
        foreach (var item in outsByNumbers)
        {
            if (outsByPlayerAndMatchDetail.ContainsKey(item.Key)) outsByPlayerAndMatchDetail[item.Key] += item.Value;
            else outsByPlayerAndMatchDetail[item.Key] = item.Value;
        }
    }

    private void InternalProcessOutsOfPlayer(Data.Entities.Ticket ticket, Dictionary<long, Dictionary<long, decimal>> playerOuts, decimal differentPlayerPayout)
    {
        if (!playerOuts.TryGetValue(ticket.PlayerId, out Dictionary<long, decimal> playerOutsDetail))
        {
            playerOutsDetail = new Dictionary<long, decimal>();
            playerOuts[ticket.PlayerId] = playerOutsDetail;
        }
        if (playerOutsDetail.ContainsKey(ticket.MatchId)) playerOutsDetail[ticket.MatchId] += differentPlayerPayout;
        else playerOutsDetail[ticket.MatchId] = differentPlayerPayout;
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