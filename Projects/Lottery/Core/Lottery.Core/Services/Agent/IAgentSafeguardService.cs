using HnMicro.Core.Scopes;
using Lottery.Core.Models.Auth;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentSafeguardService : IScopedDependency
    {
        Task ResetSercurityCode(ResetSecurityCodeModel model);
        Task ResetPassword(ResetPasswordModel model);
        Task ResetLoginUserPassword(string password, string confirmPassword);
        Task ResetLoginUserSercurityCode(string securityCode, string confirmSecurityCode);
    }
}
