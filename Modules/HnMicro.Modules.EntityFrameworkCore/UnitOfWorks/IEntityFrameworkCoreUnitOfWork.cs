using HnMicro.Framework.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HnMicro.Modules.EntityFrameworkCore.UnitOfWorks
{
    public interface IEntityFrameworkCoreUnitOfWork<TContext> : IUnitOfWork, IUnitOfWorkAsync
        where TContext : DbContext
    {
        T GetRepository<T>();
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
