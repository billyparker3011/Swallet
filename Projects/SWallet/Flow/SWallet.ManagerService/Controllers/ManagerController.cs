using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models;
using SWallet.Core.Services.Manager;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Controllers
{
    public class ManagerController : HnControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetManagers([FromQuery] GetManagersRequest request)
        {
            return Ok(OkResponse.Create(await _managerService.GetManagers(new GetManagersModel
            {
                SearchTerm = request.SearchTerm,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            })));
        }

        [HttpPost]
        public async Task<IActionResult> CreateManager([FromBody] CreateManagerRequest request)
        {
            await _managerService.CreateManager(new CreateManagerModel
            {
                Username = request.Username,
                FullName = request.FullName,
                Password = request.Password
            });
            return Ok();
        }
    }
}