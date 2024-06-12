using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class OutsController : HnControllerBase
    {
        private readonly IPlayerTicketService _playerTicketService;

        public OutsController(IPlayerTicketService playerTicketService)
        {
            _playerTicketService = playerTicketService;
        }

        [HttpGet("{playerId:long}")]
        public async Task<IActionResult> GetOuts(long playerId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetPlayerOuts(playerId)));
        }
    }
}