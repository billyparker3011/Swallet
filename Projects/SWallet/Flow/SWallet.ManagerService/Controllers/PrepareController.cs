using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWallet.ManagerService.Models.Prepare;
using SWallet.ManagerService.Requests.Prepare;
using SWallet.ManagerService.Services.Prepare;

namespace SWallet.ManagerService.Controllers
{
    [AllowAnonymous]
    public class PrepareController : HnControllerBase
    {
        private readonly IPrepareService _prepareService;

        public PrepareController(IPrepareService prepareService)
        {
            _prepareService = prepareService;
        }

        [HttpGet("1/initial-roles")]
        public async Task<IActionResult> InitialRoles()
        {
            return Ok(OkResponse.Create(await _prepareService.InitialRoles()));
        }

        [HttpGet("2/initial-customer-levels")]
        public async Task<IActionResult> InitialCustomerLevels()
        {
            return Ok(OkResponse.Create(await _prepareService.InitialCustomerLevels()));
        }

        [HttpPost("50/create-root-manager")]
        public async Task<IActionResult> CreateRootManager([FromBody] CreateRootManagerRequest request)
        {
            return Ok(OkResponse.Create(await _prepareService.CreateRootManager(new CreateRootManagerModel
            {
                LengthOfUsername = request.LengthOfUsername,
                LengthOfPassword = request.LengthOfPassword
            })));
        }

        [HttpGet("100/initial-features-permissions")]
        public async Task<IActionResult> InitialFeaturesAndPermissions()
        {
            return Ok(OkResponse.Create(await _prepareService.InitialFeaturesAndPermissions()));
        }

        [HttpGet("200/initial-settings")]
        public async Task<IActionResult> InitialSettings()
        {
            return Ok(OkResponse.Create(await _prepareService.InitialSettings()));
        }

        [HttpGet("201/initial-manual-payment")]
        public async Task<IActionResult> InitialManualPayment()
        {
            return Ok(OkResponse.Create(await _prepareService.InitialManualPayment()));
        }
    }
}