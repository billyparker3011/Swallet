using System.Linq.Expressions;

namespace HnMicro.Framework.Data.Repositories
{
    public interface IRepositoryAsync<K, T> where T : class
    {
        Task AddAsync(T item);
        Task AddRangeAsync(List<T> items);

        Task<T> FindByIdAsync(K id);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync();
        Task<int> CountByAsync(Expression<Func<T, bool>> predicate);
        Task<long> CountLongAsync();
        Task<long> CountLongByAsync(Expression<Func<T, bool>> predicate);

        Task<PagingResult<T>> PagingByAsync(IQueryable<T> query, int currentPage, int rowPerPage);
        Task<PagingResult<T>> PagingByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int currentPage, int rowPerPage);
    }
}
