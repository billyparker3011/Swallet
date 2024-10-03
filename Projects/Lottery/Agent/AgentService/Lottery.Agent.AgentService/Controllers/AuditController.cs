using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Audit;
using Lottery.Core.Models.Audit;
using Lottery.Core.Services.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class AuditController : HnControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("audits/{type:int}")]
        public async Task<IActionResult> GetAuditsByType([FromRoute] int type, [FromQuery] GetAuditsByTypeRequest request)
        {
            var result = await _auditService.GetAuditsByType(new GetAuditsByTypeModel
            {
                Type = type,
                SearchTerm = request.SearchTerm,
                SearchBetKind = request.SearchBetKind,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo
            });
            return Ok(OkResponse.Create(result.Audits, result.Metadata));
        }
    }
}
