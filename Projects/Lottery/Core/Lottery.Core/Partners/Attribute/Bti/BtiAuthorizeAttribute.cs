using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Attribute.Bti
{
    public class BtiAuthorizeAttribute : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public BtiAuthorizeAttribute(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            )
            : base(options, logger, encoder, clock)
        {

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            var identity = new ClaimsIdentity(null, nameof(BtiAuthorizeAttribute));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
    }
}
