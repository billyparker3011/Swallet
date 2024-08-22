using Azure.Core;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using static Lottery.Core.Helpers.PartnerHelper;

namespace Lottery.Core.Partners.Attribute.CA
{
    public class CasinoAuthorizeAttribute : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ICasinoRequestService _casinoRequestService;

        public CasinoAuthorizeAttribute(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ICasinoRequestService casinoRequestService
            )
            : base(options, logger, encoder, clock)
        {
            _casinoRequestService = casinoRequestService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var code = await _casinoRequestService.ValidateHeader(Request);
            if(code != CasinoReponseCode.Success)
            {
                await RespondWithCustomModel(code);
                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(), Scheme.Name));
            }

            var identity = new ClaimsIdentity(null, nameof(CasinoAuthorizeAttribute));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }

        private async Task RespondWithCustomModel(int code)
        {
            var response = new CasinoReponseModel(code);

            Response.StatusCode = StatusCodes.Status200OK;
            Response.ContentType = "application/json";
            var jsonResponse = JsonConvert.SerializeObject(response);
            await Response.WriteAsync(jsonResponse);
        }
    }

}
