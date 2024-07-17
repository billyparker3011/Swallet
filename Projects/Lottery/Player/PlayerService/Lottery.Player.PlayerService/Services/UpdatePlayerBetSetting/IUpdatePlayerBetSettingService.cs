using HnMicro.Core.Scopes;

namespace Lottery.Player.PlayerService.Services.UpdatePlayerBetSetting
{
    public interface IUpdatePlayerBetSettingService : ISingletonDependency, IDisposable
    {
        Task Start();
    }
}
