using HnMicro.Framework.Exceptions;
using Lottery.Core.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lottery.Core.Filters.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LotteryAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly List<string> _roles;

        public LotteryAuthorizeAttribute(params string[] roles)
        {
            _roles = roles.ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) return;

            var currentPermissions = context.HttpContext.User.Claims.Where(f => f.Type == ClaimConfigs.AgentClaimConfig.Permissions).Select(f => f.Value).ToList();
            if (_roles.Any() && !_roles.Any(currentPermissions.Contains))
            {
                throw new UnauthorizedException();
            }
        }
    }
}
