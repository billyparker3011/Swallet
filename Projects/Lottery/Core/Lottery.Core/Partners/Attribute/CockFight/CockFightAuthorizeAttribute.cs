using Lottery.Core.Enums.Partner;
using Lottery.Core.Repositories.BookiesSetting;
using Lottery.Core.UnitOfWorks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lottery.Core.Partners.Attribute.CockFight
{
    public class CockFightAuthorizeAttribute : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILotteryUow LotteryUow;

        public CockFightAuthorizeAttribute(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ILotteryUow lotteryUow) : base(options, logger, encoder, clock)
        {
            LotteryUow = lotteryUow;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                return AuthenticateResult.Fail("Authorization header not found.");
            
            var bookieRequestRepos = LotteryUow.GetRepository<IBookiesSettingRepository>();
            var bookieRequestSetting = await bookieRequestRepos.FindBookieSettingByType(PartnerType.GA28);
            if (bookieRequestSetting == null || bookieRequestSetting.SettingValue == null) return AuthenticateResult.Fail("Setting has not been configured yet.");
            var expectedToken = bookieRequestSetting.SettingValue.ApplicationStaticToken;

            var token = authHeader.ToString().Replace("Token ", "");

            if (token != expectedToken)
                return AuthenticateResult.Fail("Invalid token.");

            var identity = new ClaimsIdentity(null, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
