using HnMicro.Core.Scopes;
using HnMicro.Framework.Logger.Models;

namespace HnMicro.Modules.LoggerService.Providers
{
    public interface IProviderFactory : ISingletonDependency
    {
        void Enqueue(LogModel message);
    }
}
