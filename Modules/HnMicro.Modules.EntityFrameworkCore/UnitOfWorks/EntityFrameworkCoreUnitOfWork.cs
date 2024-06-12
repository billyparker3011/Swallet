using HnMicro.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HnMicro.Modules.EntityFrameworkCore.UnitOfWorks
{
    public class EntityFrameworkCoreUnitOfWork<TContext> : IEntityFrameworkCoreUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private readonly TContext _context;

        public EntityFrameworkCoreUnitOfWork(TContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.Database.CommitTransaction();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public T GetRepository<T>()
        {
            var typeOf = typeof(T);

            object v;
            if (!_repositories.TryGetValue(typeOf, out v))
            {
                var lastDerivedClass = typeOf.GetDerivedClass().FirstOrDefault();
                if (lastDerivedClass == null)
                {
                    throw new ArgumentNullException("lastDerivedClass");
                }

                v = Activator.CreateInstance(lastDerivedClass, _context);
                _repositories[typeOf] = v;
            }

            return (T)v;
        }

        public void Rollback()
        {
            _context.Database.RollbackTransaction();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
