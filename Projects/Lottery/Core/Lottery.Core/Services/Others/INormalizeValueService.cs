using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Others
{
    public interface INormalizeValueService : ISingletonDependency
    {
        decimal Normalize(decimal value);
    }
}
