using HnMicro.Framework.Data.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HnMicro.Modules.InMemory.Repositories
{
    public abstract class InMemoryRepository<K, T> : IInMemoryRepository<K, T> where T : class
    {
        protected ConcurrentDictionary<K, T> Items = new();

        public void Add(T item)
        {
            InternalTryAddOrUpdate(item);
        }

        public void AddRange(List<T> items)
        {
            foreach (var item in items)
            {
                InternalTryAddOrUpdate(item);
            }
        }

        public int Count()
        {
            return Items.Count;
        }

        public int CountBy(Expression<Func<T, bool>> predicate)
        {
            return FindBy(predicate).Count();
        }

        public long CountLong()
        {
            return Items.LongCount();
        }

        public long CountLongBy(Expression<Func<T, bool>> predicate)
        {
            return FindBy(predicate).LongCount();
        }

        public void Delete(T item)
        {
            InternalTryRemove(item);
        }

        public void DeleteById(K id)
        {
            Items.TryRemove(id, out _);
        }

        public void DeleteByIds(List<K> ids)
        {
            foreach (var id in ids) DeleteById(id);
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return Items.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public T FindById(K id)
        {
            T v;
            if (!Items.TryGetValue(id, out v)) return default(T);
            return v;
        }

        public IEnumerable<T> GetAll()
        {
            return Items.Values.AsQueryable().AsEnumerable();
        }

        public PagingResult<T> PagingBy(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, int currentPage, int rowPerPage)
        {
            var query = FindBy(predicate).AsQueryable();

            return new PagingResult<T>
            {
                Items = orderBy(query)
                    .Skip(rowPerPage * currentPage)
                    .Take(rowPerPage)
                    .AsEnumerable(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = query.Count(),
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
                    .Take(rowPerPage)
                    .AsEnumerable(),
                Metadata = new PagingMetadata
                {
                    NoOfRowsPerPage = rowPerPage,
                    NoOfRows = query.Count(),
                    Page = currentPage
                }
            };
        }

        public void Update(T item)
        {
            InternalTryAddOrUpdate(item);
        }

        protected abstract void InternalTryAddOrUpdate(T item);

        protected abstract void InternalTryRemove(T item);
    }
}
