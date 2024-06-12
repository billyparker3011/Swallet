using System.Linq.Expressions;

namespace HnMicro.Framework.Data.Repositories
{
    public interface IQueryableRepository<T> where T : class
    {
        IQueryable<T> FindQueryBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindQuery();
    }
}
