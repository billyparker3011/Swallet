using HnMicro.Framework.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HnMicro.Modules.EntityFrameworkCore.Repositories
{
    public class EntityFrameworkCoreRepository<K, T, TContext> : IEntityFrameworkCoreRepository<K, T, TContext>
        where T : class
        where TContext : DbContext
    {
        protected TContext Context;
        protected DbSet<T> DbSet;

        public EntityFrameworkCoreRepository(TContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public void Add(T item)
        {
            DbSet.Add(item);
        }

        public async Task AddAsync(T item)
        {
            await DbSet.AddAsync(item);
        }

        public void AddRange(List<T> items)
        {
            DbSet.AddRange(items);
        }

        public async Task AddRangeAsync(List<T> items)
        {
            await DbSet.AddRangeAsync(items);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        public long CountLong()
        {
            return DbSet.LongCount();
        }

        public async Task<long> CountLongAsync()
        {
            return await DbSet.LongCountAsync();
        }

        public int CountBy(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public long CountLongBy(Expression<Func<T, bool>> predicate)
        {
            return DbSet.LongCount(predicate);
        }

        public async Task<int> CountByAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        public async Task<long> CountLongByAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.LongCountAsync(predicate);
        }

        public void Delete(T item)
        {
            Context.Set<T>().Remove(item);
        }

        public void DeleteById(K id)
        {
            var entity = Context.Set<T>().Find(id);
            if (entity == null) return;
            Context.Set<T>().Remove(entity);
        }

        public void DeleteByIds(List<K> ids)
        {
            foreach (var id in ids) DeleteById(id);
        }

        public void Update(T item)
        {
            Context.Entry(item).State = EntityState.Modified;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public T FindById(K id)
        {
            return DbSet.Find(id);
        }

        public async Task<T> FindByIdAsync(K id)
        {
            return await DbSet.FindAsync(id);
        }

        public IQueryable<T> FindQuery()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<T> FindQueryBy(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public PagingResult<T> PagingBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int currentPage, int rowPerPage)
        {
            var query = FindQueryBy(predicate);

            return new PagingResult<T>
            {
                Items = orderBy(query)
                    .Skip(rowPerPage * currentPage)
                    .Take(rowPerPage).AsEnumerable(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = query.Count(),
                    Page = currentPage
                }
            };
        }

        public async Task<PagingResult<T>> PagingByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int currentPage, int rowPerPage)
        {
            var query = DbSet.Where(predicate);

            return new PagingResult<T>
            {
                Items = await orderBy(query)
                                .Skip(rowPerPage * currentPage)
                                .Take(rowPerPage).ToListAsync(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = await query.CountAsync(),
                    Page = currentPage
                }
            };
        }

        public PagingResult<T> PagingBy(IQueryable<T> query, int currentPage, int rowPerPage)
        {
            return new PagingResult<T>
            {
                Items = query
                    .Skip(rowPerPage * currentPage)
                    .Take(rowPerPage).AsEnumerable(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = query.Count(),
                    Page = currentPage
                }
            };
        }

        public async Task<PagingResult<T>> PagingByAsync(IQueryable<T> query, int currentPage, int rowPerPage)
        {
            return new PagingResult<T>
            {
                Items = await query
                            .Skip(rowPerPage * currentPage)
                            .Take(rowPerPage).ToListAsync(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = await query.CountAsync(),
                    Page = currentPage
                }
            };
        }

        public IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }
    }
}
