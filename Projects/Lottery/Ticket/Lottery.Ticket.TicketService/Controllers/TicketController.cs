using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Services.CockFight;
using Lottery.Core.Services.Partners.CA;
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
        private readonly ICockFightPlayerTicketService _cockFightPlayerTicketService;
        private readonly ICasinoTicketService _casinoTicketService;

        public TicketController(ITicketService ticketService, IPlayerTicketService playerTicketService, ICompletedMatchService completedMatchService,
            IAdvancedSearchTicketsService advancedSearchTicketsService, ICockFightPlayerTicketService cockFightPlayerTicketService, ICasinoTicketService casinoTicketService)
        {
            _ticketService = ticketService;
            _playerTicketService = playerTicketService;
            _completedMatchService = completedMatchService;
            _advancedSearchTicketsService = advancedSearchTicketsService;
            _cockFightPlayerTicketService = cockFightPlayerTicketService;
            _casinoTicketService = casinoTicketService;
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

        [HttpPost("process-v2")]
        public async Task<IActionResult> ProcessV2([FromBody] ProcessTicketV2Request request)
        {
            await _ticketService.ProcessV2(new ProcessTicketV2Model
            {
                MatchId = request.MatchId,
                DontAskWhenOddsChange = request.DontAskWhenOddsChange,
                Details = request.Details.Select(f => new ProcessTicketDetailV2Model
                {
                    BetKindId = f.BetKindId,
                    ChannelId = f.ChannelId,
                    Numbers = f.Numbers.Select(f1 => new NumberDetailModel
                    {
                        Number = f1.Number,
                        Odd = f1.Odd,
                        Point = f1.Point
                    }).ToList()
                }).ToList()
            });
            return Ok();
        }

        [HttpPost("process-mixed")]
        public async Task<IActionResult> ProcessMixed([FromBody] ProcessMixedTicketRequest request)
        {
            //  Xien 18A+B; Xien (MB)
            await _ticketService.ProcessMixed(new ProcessMixedTicketModel
            {
                BetKindId = request.BetKindId,
                MatchId = request.MatchId,
                Numbers = request.Numbers,
                Points = request.Points
            });
            return Ok();
        }

        [HttpPost("process-mixed-v2")]
        public async Task<IActionResult> ProcessMixedV2([FromBody] ProcessMixedTicketV2Request request)
        {
            //  Xien (MT + MN)
            await _ticketService.ProcessMixedV2(new ProcessMixedTicketV2Model
            {
                BetKindId = request.BetKindId,
                MatchId = request.MatchId,
                Numbers = request.Numbers,
                Points = request.Points,
                ChannelIds = request.ChannelIds
            });
            return Ok();
        }

        [HttpGet("running-tickets")]
        public async Task<IActionResult> RunningTickets()
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetTicketsAsBetList()));
        }

        [HttpGet("cock-fight/running-tickets")]
        public async Task<IActionResult> CockFightRunningTickets()
        {
            return Ok(OkResponse.Create(await _cockFightPlayerTicketService.GetCockFightTicketsAsBetList()));
        }

        [HttpGet("casino/running-tickets")]
        public async Task<IActionResult> CasinoRunningTickets()
        {
            return Ok(OkResponse.Create(await _casinoTicketService.GetCasinoTicketsAsBetList()));
        }

        [HttpGet("refund-reject-tickets")]
        public async Task<IActionResult> RefundRejectTickets()
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetRefundRejectTickets()));
        }

        [HttpGet("cock-fight/refund-reject-tickets")]
        public async Task<IActionResult> CockFightRefundRejectTickets()
        {
            return Ok(OkResponse.Create(await _cockFightPlayerTicketService.GetCockFightRefundRejectTickets()));
        }

        [HttpGet("casino/refund-reject-tickets")]
        public async Task<IActionResult> CasinoRefundRejectTickets()
        {
            return Ok(OkResponse.Create(await _casinoTicketService.GetCasinoRefundRejectTickets()));
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
        public IActionResult DraftCompletedTicketsByMatch([FromRoute] long matchId, [FromQuery] int? regionId, [FromQuery] int? channelId)
        {
            _completedMatchService.Enqueue(new CompletedMatchInQueueModel
            {
                MatchId = matchId,
                RegionId = regionId,
                ChannelId = channelId,
                IsDraft = true
            });
            return Ok();
        }

        [HttpGet("{matchId:long}/completed-tickets"), LotteryAuthorize(Permission.Management.Matches)]
        public IActionResult CompletedTicketsByMatch([FromRoute] long matchId, [FromQuery] int? regionId, [FromQuery] int? channelId)
        {
            _completedMatchService.Enqueue(new CompletedMatchInQueueModel
            {
                MatchId = matchId,
                IsDraft = false,
                Recalculation = false,
                RegionId = regionId,
                ChannelId = channelId
            });
            return Ok();
        }

        [HttpGet("{matchId:long}/completed-tickets-onemore"), LotteryAuthorize(Permission.Management.Matches)]
        public IActionResult CompletedTicketsOneMoreByMatch([FromRoute] long matchId, [FromQuery] int? regionId, [FromQuery] int? channelId)
        {
            _completedMatchService.Enqueue(new CompletedMatchInQueueModel
            {
                MatchId = matchId,
                IsDraft = false,
                Recalculation = true,
                RegionId = regionId,
                ChannelId = channelId
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