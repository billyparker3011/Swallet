using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.CockFight.GetCockFightPlayerWinlossDetail;
using Lottery.Core.Models.Winlose;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.CockFight;
using Lottery.Core.Services.Partners.CA;
using Lottery.Core.Services.Ticket;
using Lottery.Ticket.TicketService.Requests.Winlose;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class WinloseController : HnControllerBase
    {
        private readonly IPlayerTicketService _playerTicketService;
        private readonly IBroadCasterTicketService _broadCasterTicketService;
        private readonly ICockFightPlayerTicketService _cockFightPlayerTicketService;
        private readonly ICasinoPlayerTicketService _casinoPlayerTicketService;

        public WinloseController(IPlayerTicketService playerTicketService, IBroadCasterTicketService broadCasterTicketService, ICockFightPlayerTicketService cockFightPlayerTicketService, ICasinoPlayerTicketService casinoPlayerTicketService)
        {
            _playerTicketService = playerTicketService;
            _broadCasterTicketService = broadCasterTicketService;
            _cockFightPlayerTicketService = cockFightPlayerTicketService;
            _casinoPlayerTicketService = casinoPlayerTicketService;
        }

        [HttpGet("{playerId:long}/winlose-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetPlayerWinloseDetail([FromRoute] long playerId, [FromQuery] WinloseDetailRequest request)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetPlayerWinloseDetail(new WinloseDetailQueryModel
            {
                TargetId = playerId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                SelectedDraft = request.SelectedDraft
            })));
        }

        [HttpGet("broad-caster/{betkindId:long}/winlose-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetBroadCasterWinloseDetail([FromRoute] long betkindId, [FromQuery] WinloseDetailRequest request)
        {
            return Ok(OkResponse.Create(await _broadCasterTicketService.GetBroadCasterWinloseDetail(new WinloseDetailQueryModel
            {
                TargetId = betkindId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                SelectedDraft = request.SelectedDraft
            })));
        }

        [HttpGet("{ticketId:long}/ticket-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> DetailTicket(long ticketId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetDetailTicket(ticketId, false)));
        }

        [HttpGet("cock-fight/{playerId:long}/winlose-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetCockFightPlayerWinloseDetail([FromRoute] long playerId, [FromQuery] WinloseDetailRequest request)
        {
            return Ok(OkResponse.Create(await _cockFightPlayerTicketService.GetCockFightPlayerWinloseDetail(new GetCockFightPlayerWinlossDetailModel
            {
                PlayerId = playerId,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            })));
        }

        [HttpGet("casino/{playerId:long}/winlose-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetCasinoPlayerWinloseDetail([FromRoute] long playerId, [FromQuery] WinloseDetailRequest request)
        {
            return Ok(OkResponse.Create(await _casinoPlayerTicketService.GetCasinoPlayerWinloseDetail(new GetCasinoPlayerWinlossDetailModel
            {
                PlayerId = playerId,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            })));
        }
    }
}