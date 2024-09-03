using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.CockFight;
using Lottery.Core.Services.Partners.CA;
using Lottery.Core.Services.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Ticket.TicketService.Controllers
{
    public class OutsController : HnControllerBase
    {
        private readonly IPlayerTicketService _playerTicketService;
        private readonly IBroadCasterTicketService _broadCasterTicketService;
        private readonly ICockFightPlayerTicketService _cockFightPlayerTicketService;
        private readonly ICasinoPlayerTicketService _casinoPlayerTicketService;

        public OutsController(IPlayerTicketService playerTicketService, IBroadCasterTicketService broadCasterTicketService, ICockFightPlayerTicketService cockFightPlayerTicketService,
            ICasinoPlayerTicketService casinoPlayerTicketService)
        {
            _playerTicketService = playerTicketService;
            _broadCasterTicketService = broadCasterTicketService;
            _cockFightPlayerTicketService = cockFightPlayerTicketService;
            _casinoPlayerTicketService = casinoPlayerTicketService;
        }

        [HttpGet("{playerId:long}")]
        public async Task<IActionResult> GetOuts(long playerId)
        {
            return Ok(OkResponse.Create(await _playerTicketService.GetPlayerOuts(playerId)));
        }

        [HttpGet("broad-caster/{betkindId:int}")]
        public async Task<IActionResult> GetBroadCasterOuts(int betkindId)
        {
            return Ok(OkResponse.Create(await _broadCasterTicketService.GetBroadCasterOuts(betkindId)));
        }

        [HttpGet("cock-fight/{playerId:long}")]
        public async Task<IActionResult> GetCockFightPlayerOuts(long playerId)
        {
            return Ok(OkResponse.Create(await _cockFightPlayerTicketService.GetCockFightPlayerOuts(playerId)));
        }

        [HttpGet("casino/{playerId:long}")]
        public async Task<IActionResult> GetCasinoPlayerOuts(long playerId)
        {
            return Ok(OkResponse.Create(await _casinoPlayerTicketService.GetCasinoPlayerOuts(playerId)));
        }
    }
}