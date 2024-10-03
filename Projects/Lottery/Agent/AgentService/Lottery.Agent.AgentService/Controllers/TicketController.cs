using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Ticket;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class TicketController : HnControllerBase
    {
        private readonly IAgentTicketService _agentTicketService;

        public TicketController(IAgentTicketService agentTicketService)
        {
            _agentTicketService = agentTicketService;
        }

        [HttpGet("latest-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> LatestTickets([FromQuery] QueryAdvance request)
        {
            var result = await _agentTicketService.LatestTickets(new Core.Models.Ticket.AgentTicketModel
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(result.Items, result.Metadata));
        }

        [HttpGet("refund-reject-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> RefundRejctTickets([FromQuery] QueryAdvance request)
        {
            var result = await _agentTicketService.GetRefundRejectTickets(new Core.Models.Ticket.AgentTicketModel
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
