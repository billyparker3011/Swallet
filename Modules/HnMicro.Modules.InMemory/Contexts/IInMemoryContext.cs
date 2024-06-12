using HnMicro.Core.Scopes;

namespace HnMicro.Modules.InMemory.Contexts
{
    public interface IInMemoryContext : ISingletonDependency
    {
        T GetRepository<T>();
    }
}
