using System.Reflection;

namespace HnMicro.Core.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetTypesFromAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());
        }

        public static IEnumerable<Type> GetDerivedClass(this Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface && p.IsClass);
        }

        public static IEnumerable<Type> GetAllClasses()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                            .SelectMany(s => GetLoadableTypes(s))
                                            .Where(p => !p.IsInterface && !p.IsAbstract && p.IsClass);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
