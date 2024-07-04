using HnMicro.Core.Scopes;

namespace Lottery.Tools.AdjustOddsService.Services.PubSub
{
    public interface IInternalSubscribeService : ISingletonDependency
    {
        void Start();
    }
}
