using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoTicketController : HnControllerBase
    {
        private readonly ICasinoAgentTicketService _casinoAgentTicketService;
        public CasinoTicketController(ICasinoAgentTicketService casinoAgentTicketService)
        {
            _casinoAgentTicketService = casinoAgentTicketService;
        }

        [HttpGet("latest-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> CasinoLatestTickets([FromQuery] QueryAdvance request)
        {
            var result = await _casinoAgentTicketService.GetCasinoLatestTickets(new Core.Models.Ticket.AgentTicketModel
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            });
            return Ok(OkResponse.Create(result.Items, result.Metadata));
        }

        [HttpGet("refund-reject-tickets"), LotteryAuthorize(Permission.BetList.BetLists)]
        public async Task<IActionResult> GetCasinoRefundRejctTickets([FromQuery] QueryAdvance request)
        {
            var result = await _casinoAgentTicketService.GetCasinoRefundRejectTickets(new Core.Models.Ticket.AgentTicketModel
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
