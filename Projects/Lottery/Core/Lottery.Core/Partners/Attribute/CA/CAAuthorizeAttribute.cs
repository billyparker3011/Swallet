using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Lottery.Core.Partners.Attribute.CA
{
    public class CAAuthorizeAttribute : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly ICAService _caService;

        public CAAuthorizeAttribute(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            //,ICAService caService
            )
            : base(options, logger, encoder, clock)
        {
           // _caService = caService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization Header Not Found."));
            }

            if (!Request.Headers.ContainsKey("Date"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Date Header Not Found."));
            }

            var identity = new ClaimsIdentity(null , nameof(CAAuthorizeAttribute));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));


        }
    }

}
