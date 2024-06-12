using Lottery.Core.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Lottery.Core.Filters
{
    public class PlayerFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //  TODO: OnActionExecuted
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Microsoft.Extensions.Primitives.StringValues sToken;
            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderNames.Authorization, out sToken)) return;

            var token = sToken.ToString();
            if (string.IsNullOrEmpty(token)) return;

            var claims = new List<Claim>();
            var ok = TryParse(token, claims);
            if (!ok || claims.Count == 0) return;

            //  TODO Don't setup private info in Token. We only setup identifier user and get info from caching and set user in HttpContext...
            var claimsIdentity = new ClaimsIdentity(claims);
            context.HttpContext.User = new GenericPrincipal(claimsIdentity, null);
        }

        private bool TryParse(string token, List<Claim> claims)
        {
            if (string.IsNullOrEmpty(token)) return false;
            if (token.StartsWith(JwtBearerDefaults.AuthenticationScheme) || token.StartsWith($"{JwtBearerDefaults.AuthenticationScheme} "))
            {
                token = token.Replace(JwtBearerDefaults.AuthenticationScheme, "")
                    .Replace($"{JwtBearerDefaults.AuthenticationScheme} ", "")
                    .Trim();
            }

            var handler = new JwtSecurityTokenHandler();
            var tokenContent = handler.ReadJwtToken(token);
            if (tokenContent == null) return false;

            var sPlayerId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.PlayerClaimConfig.PlayerId);
            var username = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.Username);
            var sRoleId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.RoleId);
            var sAgentId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.AgentId);
            var sMasterId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.MasterId);
            var sSupermasterId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.SupermasterId);
            if (sPlayerId == null || username == null || sRoleId == null || sAgentId == null || sMasterId == null || sSupermasterId == null) return false;
            if (!long.TryParse(sPlayerId.Value, out long playerId) ||
                string.IsNullOrEmpty(username.Value) ||
                !int.TryParse(sRoleId.Value, out int roleId) ||
                !long.TryParse(sAgentId.Value, out long agentId) ||
                !long.TryParse(sMasterId.Value, out long masterId) ||
                !long.TryParse(sSupermasterId.Value, out long supermasterId)) return false;

            claims.AddRange(tokenContent.Claims);
            return true;
        }
    }
}
