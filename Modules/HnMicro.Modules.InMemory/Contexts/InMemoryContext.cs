using HnMicro.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace HnMicro.Modules.InMemory.Contexts
{
    public class InMemoryContext : IInMemoryContext
    {
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public T GetRepository<T>()
        {
            var type = typeof(T);

            object v;
            if (!_repositories.TryGetValue(type, out v))
            {
                var lastDerivedClass = type.GetDerivedClass().FirstOrDefault();
                if (lastDerivedClass == null)
                {
                    throw new ArgumentNullException("lastDerivedClass");
                }

                v = Activator.CreateInstance(lastDerivedClass);
                _repositories[type] = v;
            }

            return (T)v;
        }
    }
}
