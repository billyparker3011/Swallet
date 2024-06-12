using HnMicro.Core.Scopes;

namespace HnMicro.Modules.InMemory.UnitOfWorks
{
    public interface IInMemoryUnitOfWork : ISingletonDependency
    {
        T GetRepository<T>();
    }
}
