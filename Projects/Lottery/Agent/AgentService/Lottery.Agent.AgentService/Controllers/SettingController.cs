using HnMicro.Framework.Controllers;
using Lottery.Agent.AgentService.Requests.Setting.ProcessTicket;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Services.Setting;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Setting.BaseRoute)]
    public class SettingController : HnControllerBase
    {
        private readonly IProcessTicketSettingService _processTicketSettingService;

        public SettingController(IProcessTicketSettingService processTicketSettingService)
        {
            _processTicketSettingService = processTicketSettingService;
        }

        #region Process Ticket Setting
        [HttpPost("process-ticket/scan-waiting-ticket")]
        public async Task<IActionResult> UpdateScanWaitingTicketSetting([FromBody] ScanWaitingTicketSettingRequest request)
        {
            await _processTicketSettingService.UpdateScanWaitingTicketSetting(new ScanWaitingTicketSettingModel
            {
                Live = new ScanWaitingTicketSettingDetailModel
                {
                    AllowAccepted = request.Live.AllowAccepted,
                    IntervalAcceptedInSeconds = request.Live.IntervalAcceptedInSeconds
                },
                NoneLive = new ScanWaitingTicketSettingDetailModel
                {
                    AllowAccepted = request.NoneLive.AllowAccepted,
                    IntervalAcceptedInSeconds = request.NoneLive.IntervalAcceptedInSeconds
                }
            });
            return Ok();
        }

        [HttpGet("process-ticket/scan-waiting-ticket")]
        public async Task<IActionResult> GetScanWaitingTicketSetting()
        {
            return Ok(await _processTicketSettingService.GetScanWaitingTicketSetting());
        }

        [HttpPost("process-ticket/validation-prize")]
        public async Task<IActionResult> UpdateValidationPrizeSetting([FromBody] ValidationPrizeSettingRequest request)
        {
            await _processTicketSettingService.UpdateValidationPrizeSetting(new ValidationPrizeSettingModel
            {
                BetKindId = request.BetKindId,
                Prize = request.Prize
            });
            return Ok();
        }

        [HttpGet("process-ticket/validation-prize/bet-kinds/{betKindId:int}")]
        public async Task<IActionResult> GetValidationPrizeSetting([FromRoute] int betKindId)
        {
            return Ok(await _processTicketSettingService.GetValidationPrizeSetting(betKindId));
        }
        #endregion
    }
}
