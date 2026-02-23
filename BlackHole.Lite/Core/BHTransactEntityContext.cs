using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// Contains the required settings of the Entity so the
    /// Data Provider's Extension methods can use it.
    /// </summary>
    /// <typeparam name="T">BlackHoleEntity</typeparam>
    public class BHTransactEntityContext<T>
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }

        internal List<IncludePart> Includes { get; private set; } = new();

        internal BlackHoleTransaction _transatcion { get; private set; }

        internal BHTransactEntityContext(bool useActivator, string tableName, List<string> tableColumns,
            string propNames, string propParams, string updateParams, BlackHoleTransaction transaction)
        {
            _transatcion = transaction;
            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public class BHTransactEntityContext<T, G> where T : BHEntity where G : BHEntity
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }

        internal List<IncludePart> Includes { get; private set; }

        internal BlackHoleTransaction _transatcion { get; private set; }

        internal BHTransactEntityContext(bool useActivator, string tableName, List<string> tableColumns,
            string propNames, string propParams, string updateParams, List<IncludePart> includes, BlackHoleTransaction transaction)
        {

            Includes = includes;
            _transatcion = transaction;
            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
        }
    }
}
