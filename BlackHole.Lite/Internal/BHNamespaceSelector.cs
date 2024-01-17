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
            return ass.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetInitialViews(Assembly ass)
        {
            Type type = typeof(IBHInitialViews);
            return ass.GetTypes().Where(p => type.IsAssignableFrom(p)).ToList();
        }

        internal List<Type> GetAllBHEntities(Assembly ass)
        {
            return ass.GetTypes().Where(t => t.BaseType == typeof(BlackHoleEntity)).ToList();
        }
    }
}
