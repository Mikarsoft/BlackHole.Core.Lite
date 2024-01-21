using BlackHole.Core;
using BlackHole.Entities;
using System.Reflection;

namespace BlackHole.Internal
{
    internal class BHNamespaceSelector
    {
        internal List<Type> GetInitialData(Assembly ass)
        {
            Type type = typeof(IBHInitialData);
            return ass.GetTypes().Where(p => p != null && type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetInitialDataInNamespace(Assembly ass, string nameSpaces)
        {
            Type type = typeof(IBHInitialData);
            return ass.GetTypes().Where(p => p != null && 
            string.Equals(p.Namespace, nameSpaces, StringComparison.Ordinal) && type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetInitialViews(Assembly ass)
        {
            Type type = typeof(IBHInitialViews);
            return ass.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetInitialViewsInNamespace(Assembly ass, string nameSpaces)
        {
            Type type = typeof(IBHInitialViews);
            return ass.GetTypes().Where(p => p != null &&
            string.Equals(p.Namespace, nameSpaces, StringComparison.Ordinal) && type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetAllBHEntities(Assembly ass)
        {
            return ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity)).ToList();
        }

        internal List<Type> GetBHEntitiesInNamespace(Assembly ass, string nameSpaces)
        {
            return ass.GetTypes().Where(t => string.Equals(t.Namespace, nameSpaces, StringComparison.Ordinal) 
            && t.BaseType == typeof(BlackHoleEntity)).ToList();
        }
    }
}
