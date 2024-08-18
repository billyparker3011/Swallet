using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoPlayerBetSettingController : HnControllerBase
    {
        private readonly ICasinoPlayerBetSettingService _cAPlayerBetSettingService;
        public CasinoPlayerBetSettingController(ICasinoPlayerBetSettingService cAPlayerBetSettingService)
        {
            _cAPlayerBetSettingService = cAPlayerBetSettingService;
        }

        [HttpGet("bet-settings/{playerId:long}")]
        public async Task<IActionResult> GetPlayerBetSettings([FromRoute] long playerId)
        {
            if (playerId < 1) return NotFound();
            var playerBetSettings = await _cAPlayerBetSettingService.GetPlayerBetSettingsWithIncludeAsync(playerId);
            return Ok(OkResponse.Create(playerBetSettings?.Select(c => new CasinoPlayerBetSettingModel(c)).ToList()));
        }

        [HttpGet("bet-settings")]
        public async Task<IActionResult> GetAllPlayerBetSettings()
        {
            return Ok(OkResponse.Create(await _cAPlayerBetSettingService.GetAllPlayerBetSettingsAsync()));
        }

        [HttpGet("bet-setting/{id:long}")]
        public async Task<IActionResult> GetPlayerBetSetting([FromRoute] long id)
        {
            if (id < 1) return NotFound();
            var playerBetSetting = await _cAPlayerBetSettingService.FindPlayerBetSettingWithIncludeAsync(id);
            if (playerBetSetting == null) return NotFound();

            return Ok(OkResponse.Create(new CasinoPlayerBetSettingModel(playerBetSetting)));
        }

        [HttpPost("bet-setting")]
        public async Task<IActionResult> CreatePlayerBetSetting(CreateCasinoPlayerBetSettingModel model)
        {
            await _cAPlayerBetSettingService.CreatePlayerBetSettingAsync(model);
            return Ok();
        }

        [HttpPost("bet-settings")]
        public async Task<IActionResult> CreatePlayerBetSettings(List<CreateCasinoPlayerBetSettingModel> models)
        {
            foreach(var model in models)
            {
                await _cAPlayerBetSettingService.CreatePlayerBetSettingAsync(model);
            }
            return Ok();
        }

        [HttpPut("{id:long}/bet-setting")]
        public async Task<IActionResult> UpdatePlayerBetSetting([FromRoute] long id, UpdateCasinoPlayerBetSettingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _cAPlayerBetSettingService.UpdatePlayerBetSettingAsync(model);
            return Ok();
        }

        [HttpPut("bet-settings")]
        public async Task<IActionResult> UpdatePlayerBetSettings(List<UpdateCasinoPlayerBetSettingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _cAPlayerBetSettingService.UpdatePlayerBetSettingAsync(model);
            }

            return Ok();
        }

        [HttpDelete("{id:long}/bet-setting")]
        public async Task<IActionResult> DeletePlayerBetSetting([FromRoute] long id)
        {
            if (id < 1) return NotFound();
            await _cAPlayerBetSettingService.DeletePlayerBetSettingAsync(id);
            return Ok();
        }
    }
}
