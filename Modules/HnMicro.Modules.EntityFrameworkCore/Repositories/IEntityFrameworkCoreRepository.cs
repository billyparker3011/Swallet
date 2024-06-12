using HnMicro.Framework.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HnMicro.Modules.EntityFrameworkCore.Repositories
{
    public interface IEntityFrameworkCoreRepository<K, T, TContext> : IRepository<K, T>, IRepositoryAsync<K, T>, IQueryableRepository<T>
        where T : class
        where TContext : DbContext
    {
    }
}
