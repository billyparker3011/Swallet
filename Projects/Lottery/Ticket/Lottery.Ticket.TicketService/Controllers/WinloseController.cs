using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Winlose;
using Lottery.Core.Services.Ticket;
using Lottery.Ticket.TicketService.Requests.Winlose;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class WinloseController : HnControllerBase
    {
        private readonly IPlayerTicketService _playerTicketService;

        public WinloseController(IPlayerTicketService playerTicketService)
        {
            _playerTicketService = playerTicketService;
        }

        [HttpGet("{playerId:long}/winlose-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetPlayerWinloseDetail([FromRoute] long playerId, [FromQuery] WinloseDetailRequest request)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetPlayerWinloseDetail(new WinloseDetailQueryModel
            {
                PlayerId = playerId,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            })));
        }

        [HttpGet("{ticketId:long}/ticket-detail"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> DetailTicket(long ticketId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetDetailTicket(ticketId, false)));
        }
    }
}