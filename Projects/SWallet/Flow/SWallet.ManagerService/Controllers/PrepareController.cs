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
    }
}