using HnMicro.Framework.Data.Repositories;

namespace HnMicro.Modules.InMemory.Repositories
{
    public interface IInMemoryRepository<K, T> : IRepository<K, T> where T : class
    {

    }
}
