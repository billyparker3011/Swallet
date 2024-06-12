using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Statement;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class StatementController : HnControllerBase
    {
        private readonly IStatementService _statementService;

        public StatementController(IStatementService statementService)
        {
            _statementService = statementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatement()
        {
            return Ok(OkResponse.Create(await _statementService.GetMyStatement()));
        }
    }
}