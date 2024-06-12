using HnMicro.Framework.Configs;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using HnMicro.Framework.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HnMicro.Framework.Controllers
{
    [AllowAnonymous]
    public class ExampleController : HnControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IJwtTokenService _jwtTokenService;

        public ExampleController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("Get-Single")]
        public ActionResult<ApiResponse<ExampleInformation>> GetSingle()
        {
            var data = Enumerable.Range(1, 5).Select(index => new ExampleInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureCelsius = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .First();
            return Ok(OkResponse.Create(data));
        }

        [HttpGet("Get-Multiple")]
        public ActionResult<ApiResponse<IEnumerable<ExampleInformation>>> GetMultiple()
        {
            var data = Enumerable.Range(1, 5).Select(index => new ExampleInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureCelsius = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            return Ok(OkResponse.Create(data, new ApiResponseMetadata { NoOfRows = 1L, NoOfPages = 1, NoOfRowsPerPage = 1, Page = 1 }));
        }

        [HttpGet("Get-400")]
        public IActionResult Get400()
        {
            throw new BadRequestException();
        }

        [HttpGet("Get-403")]
        public IActionResult Get403()
        {
            throw new ForbiddenException();
        }

        [HttpGet("Get-404")]
        public IActionResult Get404()
        {
            throw new NotFoundException();
        }

        [HttpGet("Get-401")]
        public IActionResult Get401()
        {
            throw new UnauthorizedException();
        }

        [HttpGet("Get-500")]
        public IActionResult Get500()
        {
            throw new HnMicroException();
        }

        [HttpPost("Create-Token")]
        public IActionResult CreateToken([FromBody] JwtTokenRequest request)
        {
            var claims = new List<Claim>
            {
                new Claim(ExampleClaimConfigs.Username, request.Username),
                new Claim(ExampleClaimConfigs.FirstName, request.FirstName),
                new Claim(ExampleClaimConfigs.LastName, request.LastName)
            };
            return Ok(_jwtTokenService.BuildToken(claims));
        }
    }
}
