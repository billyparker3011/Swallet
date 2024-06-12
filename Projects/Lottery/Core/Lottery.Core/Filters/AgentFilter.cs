using Lottery.Core.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Lottery.Core.Filters
{
    public class AgentFilter : IActionFilter
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
            context.HttpContext.User = new GenericPrincipal(claimsIdentity, claims.Where(f => f.Type == ClaimConfigs.AgentClaimConfig.Permissions).Select(f => f.Value).ToArray());
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

            var sRoleId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.RoleId);
            var username = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.Username);
            var sAgentId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.AgentClaimConfig.AgentId);
            var sParentId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.AgentClaimConfig.ParentId);
            var sPermissions = tokenContent.Claims.Where(f => f.Type == ClaimConfigs.AgentClaimConfig.Permissions).ToList();
            if (sRoleId == null || username == null || sAgentId == null) return false;
            if (!int.TryParse(sRoleId.Value, out int roleId) ||
                string.IsNullOrEmpty(username.Value) ||
                !long.TryParse(sAgentId.Value, out long agentId) ||
                !long.TryParse(sParentId.Value, out long parentId) ||
                sPermissions.Count == 0) return false;

            claims.AddRange(tokenContent.Claims);
            return true;
        }
    }
}
