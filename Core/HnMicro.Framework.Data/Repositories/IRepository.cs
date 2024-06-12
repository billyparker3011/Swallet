using System.Linq.Expressions;

namespace HnMicro.Framework.Data.Repositories
{
    public interface IRepository<K, T> where T : class
    {
        void Add(T item);
        void AddRange(List<T> items);
        void Update(T item);
        void Delete(T item);
        void DeleteById(K id);
        void DeleteByIds(List<K> ids);

        IEnumerable<T> GetAll();

        T FindById(K id);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        int Count();
        int CountBy(Expression<Func<T, bool>> predicate);
        long CountLong();
        long CountLongBy(Expression<Func<T, bool>> predicate);

        PagingResult<T> PagingBy(IQueryable<T> query, int currentPage, int rowPerPage);
        PagingResult<T> PagingBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int currentPage, int rowPerPage);
    }
}
