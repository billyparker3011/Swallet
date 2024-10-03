using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiTicketController : HnControllerBase
    {
        private readonly IBtiAgentTicketService _btiAgentTicketService;

        public BtiTicketController(IBtiAgentTicketService btiAgentTicketService)
        {
            _btiAgentTicketService = btiAgentTicketService;
        }

        [HttpGet("latest-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> BtiLatestTickets([FromQuery] QueryAdvance request)
        {
            var result = await _btiAgentTicketService.BtiLatestTickets(new Core.Models.Ticket.AgentTicketModel
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(result.Items, result.Metadata));
        }

        [HttpGet("refund-reject-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> BtiRefundRejctTickets([FromQuery] QueryAdvance request)
        {
            var result = await _btiAgentTicketService.GetBtiRefundRejectTickets(new Core.Models.Ticket.AgentTicketModel
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
