using Lottery.Core.Configs;
using Lottery.Core.Models.Client;
using System.IdentityModel.Tokens.Jwt;

namespace Lottery.Player.OddsService.Services.Token
{
    public class ReadTokenService : IReadTokenService
    {
        public ClientPlayerModel ReadToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenContent = handler.ReadJwtToken(accessToken);
            if (tokenContent == null) return null;

            var sRoleId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.RoleId);
            var username = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.Username);
            var sPlayerId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.PlayerClaimConfig.PlayerId);
            var sAgentId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.AgentId);
            var sMasterId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.MasterId);
            var sSupermasterId = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.SupermasterId);
            var hash = tokenContent.Claims.FirstOrDefault(f => f.Type == ClaimConfigs.Hash);

            if (sRoleId == null || username == null || sPlayerId == null || sAgentId == null || sMasterId == null || sSupermasterId == null) return null;
            if (!int.TryParse(sRoleId.Value, out int roleId) ||
                string.IsNullOrEmpty(username.Value) ||
                !long.TryParse(sPlayerId.Value, out long playerId) ||
                !long.TryParse(sAgentId.Value, out long agentId) ||
                !long.TryParse(sMasterId.Value, out long masterId) ||
                !long.TryParse(sSupermasterId.Value, out long supermasterId)) return null;

            return new ClientPlayerModel
            {
                RoleId = roleId,
                UserName = username.Value,
                PlayerId = playerId,
                AgentId = agentId,
                MasterId = masterId,
                SupermasterId = supermasterId,
                Hash = hash.Value
            };
        }
    }
}
