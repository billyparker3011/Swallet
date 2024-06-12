using HnMicro.Framework.Caching.Services;

namespace HnMicro.Module.Caching.ByRedis.Services
{
    public interface IRedisCacheService : ICacheService, IRedisCacheExtension
    {

    }
}
