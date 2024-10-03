using HnMicro.Core.Scopes;
using Lottery.Core.Models.Auth;

namespace Lottery.Core.Services.Player
{
    public interface IPlayerSafeguardService : IScopedDependency
    {
        Task ChangePassword(PlayerChangePasswordModel model);
        Task ResetPassword(ResetPasswordModel model);
    }
}
