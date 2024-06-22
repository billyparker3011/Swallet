using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Services.Ticket;
using Lottery.Ticket.TicketService.Requests.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class TicketController : HnControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IPlayerTicketService _playerTicketService;
        private readonly ICompletedMatchService _completedMatchService;
        private readonly IAdvancedSearchTicketsService _advancedSearchTicketsService;

        public TicketController(ITicketService ticketService, IPlayerTicketService playerTicketService, ICompletedMatchService completedMatchService,
            IAdvancedSearchTicketsService advancedSearchTicketsService)
        {
            _ticketService = ticketService;
            _playerTicketService = playerTicketService;
            _completedMatchService = completedMatchService;
            _advancedSearchTicketsService = advancedSearchTicketsService;
        }

        [HttpPost]
        public async Task<IActionResult> Process([FromBody] ProcessTicketRequest request)
        {
            await _ticketService.Process(new ProcessTicketModel
            {
                BetKindId = request.BetKindId,
                MatchId = request.MatchId,
                Numbers = request.Numbers.Select(f => new NumberDetailModel
                {
                    Number = f.Number,
                    Point = f.Point,
                    Odd = f.Odd
                }).ToList(),
                DontAskWhenOddsChange = request.DontAskWhenOddsChange
            });
            return Ok();
        }

        [HttpPost("process-mixed")]
        public async Task<IActionResult> ProcessMixed([FromBody] ProcessMixedTicketRequest request)
        {
            await _ticketService.ProcessMixed(new ProcessMixedTicketModel
            {
                BetKindId = request.BetKindId,
                MatchId = request.MatchId,
                Numbers = request.Numbers,
                Points = request.Points
            });
            return Ok();
        }

        [HttpGet("running-tickets")]
        public async Task<IActionResult> RunningTickets()
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetTicketsAsBetList()));
        }

        [HttpGet("refund-reject-tickets")]
        public async Task<IActionResult> RefundRejectTickets()
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetRefundRejectTickets()));
        }

        [HttpGet("{matchId:long}/bet-list")]
        public async Task<IActionResult> BetList([FromRoute] long matchId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetTicketsByMatch(matchId)));
        }

        [HttpGet("{ticketId:long}")]
        public async Task<IActionResult> DetailTicket([FromRoute] long ticketId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetDetailTicket(ticketId)));
        }

        [HttpGet("{matchId:long}/load-tickets"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> LoadTicketsByMatch([FromRoute] long matchId, [FromQuery] int top = 1000)
        {
            await _ticketService.LoadTicketsByMatch(matchId, top);
            return Ok();
        }

        [HttpGet("{matchId:long}/draft-completed-tickets"), LotteryAuthorize(Permission.Management.Matches)]
        public IActionResult DraftCompletedTicketsByMatch([FromRoute] long matchId)
        {
            _completedMatchService.Enqueue(new CompletedMatchInQueueModel
            {
                MatchId = matchId,
                IsDraft = true
            });
            return Ok();
        }

        [HttpGet("{matchId:long}/completed-tickets-onemore"), LotteryAuthorize(Permission.Management.Matches)]
        public IActionResult CompletedTicketsOneMoreByMatch([FromRoute] long matchId)
        {
            _completedMatchService.Enqueue(new CompletedMatchInQueueModel
            {
                MatchId = matchId,
                IsDraft = false,
                Recalculation = true
            });
            return Ok();
        }

        [HttpGet("advanced-search"), LotteryAuthorize(Permission.Management.AdvancedTickets)]
        public async Task<IActionResult> AdvancedSearchTickets([FromQuery] AdvancedSearchTicketsRequest request)
        {
            var data = await _advancedSearchTicketsService.Search(new AdvancedSearchTicketsModel
            {
                MatchId = request.MatchId,
                RegionId = request.RegionId,
                BetKindIds = request.BetKindIds,
                ChannelId = request.ChannelId,
                LiveStates = request.LiveStates,
                TicketIds = request.TicketIds,
                Username = request.Username,
                Prizes = request.Prizes,
                Positions = request.Positions,
                ChooseNumbers = request.ChooseNumbers,
                ContainNumberOperator = request.ContainNumberOperator,
                States = request.States,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(data.Items, data.Metadata));
        }

        [HttpPost("refund-reject-tickets"), LotteryAuthorize(Permission.Management.AdvancedTickets)]
        public async Task<IActionResult> RefundRejectTickets([FromBody] RefundRejectTicketsRequest request)
        {
            await _advancedSearchTicketsService.RefundRejectTickets(request.TicketIds);
            return Ok();
        }

        [HttpPost("refund-reject-tickets-by-numbers"), LotteryAuthorize(Permission.Management.AdvancedTickets)]
        public async Task<IActionResult> RefundRejectTicketsByNumbers([FromBody] RefundRejectTicketsByNumbersRequest request)
        {
            await _advancedSearchTicketsService.RefundRejectTicketsByNumbers(request.TicketIds, request.Numbers);
            return Ok();
        }
    }
}