using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class OutsController : HnControllerBase
    {
        private readonly IPlayerTicketService _playerTicketService;
        private readonly IBroadCasterTicketService _broadCasterTicketService;

        public OutsController(IPlayerTicketService playerTicketService, IBroadCasterTicketService broadCasterTicketService)
        {
            _playerTicketService = playerTicketService;
            _broadCasterTicketService = broadCasterTicketService;
        }

        [HttpGet("{playerId:long}")]
        public async Task<IActionResult> GetOuts(long playerId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetPlayerOuts(playerId)));
        }

        [HttpGet("/broad-caster/{betkindId:int}")]
        public async Task<IActionResult> GetBroadCasterOuts(int betkindId)
        {
            return Ok(OkResponse.Create(await _broadCasterTicketService.GetBroadCasterOuts(betkindId)));
        }
    }
}