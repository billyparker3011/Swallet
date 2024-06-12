using HnMicro.Core.Scopes;

namespace HnMicro.Modules.LoggerService.Services
{
    public interface ISubscribeLoggerUsingRedisService : ISingletonDependency
    {
        void Start();
    }
}
