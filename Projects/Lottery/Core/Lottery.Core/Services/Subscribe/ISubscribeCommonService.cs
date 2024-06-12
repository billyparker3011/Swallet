using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Subscribe
{
    public interface ISubscribeCommonService : ISingletonDependency
    {
        void Start();
    }
}
