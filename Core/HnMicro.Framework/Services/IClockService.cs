using HnMicro.Core.Scopes;

namespace HnMicro.Framework.Services
{
    public interface IClockService : ISingletonDependency
    {
        DateTime GetNow();
        DateTime GetUtcNow();
        DateTime ToDateTime(long ticks);
    }
}
