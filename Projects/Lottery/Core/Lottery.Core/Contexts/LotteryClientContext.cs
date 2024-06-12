using HnMicro.Framework.Contexts;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Configs;
using Lottery.Core.Models.Client;
using Microsoft.AspNetCore.Http;

namespace Lottery.Core.Contexts
{
    public class LotteryClientContext : BaseClientContext, ILotteryClientContext
    {
        public LotteryClientContext(IHttpContextAccessor httpContext) : base(httpContext)
        {
        }

        public ClientAgentModel Agent
        {
            get
            {
                if (HttpContextAccessor.HttpContext == null)
                    throw new HnMicroException("HttpContext cannot be NULL.");

                var claimAgentId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.AgentClaimConfig.AgentId);
                if (claimAgentId == null || string.IsNullOrEmpty(claimAgentId.Value) || !long.TryParse(claimAgentId.Value, out long agentId) || agentId <= 0L)
                    throw new HnMicroException("Cannot parse AgentId.");

                var claimMasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.MasterId);
                if (claimMasterId == null || string.IsNullOrEmpty(claimMasterId.Value) || !long.TryParse(claimMasterId.Value, out long masterId))
                    throw new HnMicroException("Cannot parse MasterId.");

                var claimSupermasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.SupermasterId);
                if (claimSupermasterId == null || string.IsNullOrEmpty(claimSupermasterId.Value) || !long.TryParse(claimSupermasterId.Value, out long supermasterId))
                    throw new HnMicroException("Cannot parse SupermasterId.");

                var claimParentId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.AgentClaimConfig.ParentId);
                if (claimParentId == null || string.IsNullOrEmpty(claimParentId.Value) || !long.TryParse(claimParentId.Value, out long parentId))
                    throw new HnMicroException("Cannot parse ParentId.");

                var claimUserName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Username);
                if (claimUserName == null || string.IsNullOrEmpty(claimUserName.Value))
                    throw new HnMicroException("Cannot parse UserName.");

                var claimRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.RoleId);
                if (claimRole == null || string.IsNullOrEmpty(claimRole.Value) || !int.TryParse(claimRole.Value, out int roleId))
                    throw new HnMicroException("Cannot parse Role.");

                var claimPermissions = HttpContextAccessor.HttpContext.User.Claims.Where(f => f.Type == ClaimConfigs.AgentClaimConfig.Permissions).Select(f => f.Value).ToList();
                if (claimPermissions.Count == 0)
                    throw new HnMicroException("Cannot parse Permissions.");

                var claimHash = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Hash);
                if (claimHash == null || string.IsNullOrEmpty(claimHash.Value))
                    throw new HnMicroException("Cannot parse Hash.");

                return new ClientAgentModel
                {
                    AgentId = agentId,
                    UserName = claimUserName.Value,
                    RoleId = roleId,
                    ParentId = parentId,
                    MasterId = masterId,
                    SupermasterId = supermasterId,
                    Permissions = claimPermissions,
                    Hash = claimHash.Value
                };
            }
        }

        public ClientPlayerModel Player
        {
            get
            {
                if (HttpContextAccessor.HttpContext == null)
                    throw new HnMicroException("HttpContext cannot be NULL.");

                var claimPlayerId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.PlayerClaimConfig.PlayerId);
                if (claimPlayerId == null || string.IsNullOrEmpty(claimPlayerId.Value) || !long.TryParse(claimPlayerId.Value, out long playerId) || playerId == 0L)
                    throw new HnMicroException("Cannot parse PlayerId.");

                var claimUserName = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Username);
                if (claimUserName == null || string.IsNullOrEmpty(claimUserName.Value))
                    throw new HnMicroException("Cannot parse UserName.");

                var claimRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.RoleId);
                if (claimRole == null || string.IsNullOrEmpty(claimRole.Value) || !int.TryParse(claimRole.Value, out int roleId))
                    throw new HnMicroException("Cannot parse Role.");

                var claimAgentId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.AgentId);
                if (claimAgentId == null || string.IsNullOrEmpty(claimAgentId.Value) || !long.TryParse(claimAgentId.Value, out long agentId) || agentId <= 0L)
                    throw new HnMicroException("Cannot parse AgentId.");

                var claimMasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.MasterId);
                if (claimMasterId == null || string.IsNullOrEmpty(claimMasterId.Value) || !long.TryParse(claimMasterId.Value, out long masterId) || masterId <= 0L)
                    throw new HnMicroException("Cannot parse MasterId.");

                var claimSupermasterId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.SupermasterId);
                if (claimSupermasterId == null || string.IsNullOrEmpty(claimSupermasterId.Value) || !long.TryParse(claimSupermasterId.Value, out long supermasterId) || supermasterId <= 0L)
                    throw new HnMicroException("Cannot parse SupermasterId.");

                var claimHash = HttpContextAccessor.HttpContext.User.FindFirst(ClaimConfigs.Hash);
                if (claimHash == null || string.IsNullOrEmpty(claimHash.Value))
                    throw new HnMicroException("Cannot parse Hash.");

                return new ClientPlayerModel
                {
                    PlayerId = playerId,
                    UserName = claimUserName.Value,
                    RoleId = roleId,
                    AgentId = agentId,
                    MasterId = masterId,
                    SupermasterId = supermasterId,
                    Hash = claimHash.Value
                };
            }
        }
    }
}
