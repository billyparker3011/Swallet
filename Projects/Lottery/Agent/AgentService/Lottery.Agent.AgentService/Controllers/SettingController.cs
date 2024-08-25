using HnMicro.Framework.Controllers;
using Lottery.Agent.AgentService.Requests.Setting.BetKind;
using Lottery.Agent.AgentService.Requests.Setting.ProcessTicket;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightBookieSetting;
using Lottery.Core.Models.Setting.BetKind;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Services.CockFight;
using Lottery.Core.Services.Setting;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Setting.BaseRoute)]
    public class SettingController : HnControllerBase
    {
        private readonly IProcessTicketSettingService _processTicketSettingService;
        private readonly IBalanceTableSettingService _balanceTableSettingService;
        private readonly ICockFightService _cockFightService;

        public SettingController(IProcessTicketSettingService processTicketSettingService, IBalanceTableSettingService balanceTableSettingService, ICockFightService cockFightService)
        {
            _processTicketSettingService = processTicketSettingService;
            _balanceTableSettingService = balanceTableSettingService;
            _cockFightService = cockFightService;
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

        [HttpGet("process-ticket/channels-completed-tickets")]
        public async Task<IActionResult> GetChannelsForCompletedTicket()
        {
            return Ok(await _processTicketSettingService.GetChannelsForCompletedTicket());
        }

        [HttpPost("process-ticket/channels-completed-tickets")]
        public async Task<IActionResult> UpdateChannelsForCompletedTicket([FromBody] ChannelsForCompletedTicketRequest request)
        {
            await _processTicketSettingService.UpdateChannelsForCompletedTicket(new ChannelsForCompletedTicketModel
            {
                Items = request.Items.ToDictionary(f => f.Key, f => f.Value.Select(f1 => new ChannelsForCompletedTicketDetailModel
                {
                    DayOfWeek = f1.DayOfWeek,
                    ChannelIds = f1.ChannelIds
                }).ToList())
            });
            return Ok();
        }
        #endregion

        #region Balance Table Setting
        [HttpPost("balance-table/{betkindId:int}")]
        public async Task<IActionResult> CreateOrModifyBetKindBalanceTableSetting([FromRoute] int betKindId, [FromBody] BalanceTableRequest request)
        {
            await _balanceTableSettingService.CreateOrModifyBetKindBalanceTableSetting(betKindId, new BalanceTableModel
            {
                ByNumbers = new BalanceTableNumberDetailModel
                {
                    Numbers = request.ByNumbers.Numbers,
                    RateValues = request.ByNumbers.RateValues.Select(f => new BalanceTableRateModel
                    {
                        From = f.From,
                        To = f.To,
                        RateValue = f.RateValue,
                        Applied = f.Applied
                    }).ToList()
                },
                ForCommon = new BalanceTableCommonDetailModel
                {
                    RateValues = request.ForCommon.RateValues.Select(f => new BalanceTableRateModel
                    {
                        From = f.From,
                        To = f.To,
                        RateValue = f.RateValue,
                        Applied = f.Applied
                    }).ToList()
                }
            });
            return Ok();
        }

        [HttpGet("balance-table/{betkindId:int}")]
        public async Task<IActionResult> GetBetKindBalanceTableSetting([FromRoute] int betKindId)
        {
            return Ok(await _balanceTableSettingService.GetBetKindBalanceTableSetting(betKindId));
        }
        #endregion

        #region CockFight bookie setting
        [HttpGet("cock-fight/bookie-setting")]
        public async Task<IActionResult> GetCockFightBookieSetting()
        {
            return Ok(await _cockFightService.GetCockFightBookieSetting());
        }

        [HttpPut("cock-fight/bookie-setting")]
        public async Task<IActionResult> UpdateCockFightBookieSetting([FromBody] UpdateCockFightBookieSettingModel request)
        {
            await _cockFightService.UpdateCockFightBookieSetting(request);
            return Ok();
        }
        #endregion
    }
}
