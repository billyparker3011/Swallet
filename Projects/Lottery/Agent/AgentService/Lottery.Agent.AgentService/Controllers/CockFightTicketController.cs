using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightTicketController : HnControllerBase
    {
        private readonly ICockFightAgentTicketService _cockFightAgentTicketService;

        public CockFightTicketController(ICockFightAgentTicketService cockFightAgentTicketService)
        {
            _cockFightAgentTicketService = cockFightAgentTicketService;
        }

        [HttpGet("latest-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> CockFightLatestTickets([FromQuery] QueryAdvance request)
        {
            var result = await _cockFightAgentTicketService.CockFightLatestTickets(new Core.Models.Ticket.AgentTicketModel
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(result.Items, result.Metadata));
        }

        [HttpGet("refund-reject-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> CockFightRefundRejctTickets([FromQuery] QueryAdvance request)
        {
            var result = await _cockFightAgentTicketService.GetCockFightRefundRejectTickets(new Core.Models.Ticket.AgentTicketModel
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(result.Items, result.Metadata));
        }
    }
}
