using HnMicro.Modules.InMemory.Contexts;

namespace HnMicro.Modules.InMemory.UnitOfWorks
{
    public class InMemoryUnitOfWork : IInMemoryUnitOfWork
    {
        private readonly IInMemoryContext _inMemoryContext;

        public InMemoryUnitOfWork(IInMemoryContext inMemoryContext)
        {
            _inMemoryContext = inMemoryContext;
        }

        public T GetRepository<T>()
        {
            return _inMemoryContext.GetRepository<T>();
        }
    }
}
