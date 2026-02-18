using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using BlackHole.Entities;

namespace BlackHole.Core
{
    internal class BHEntityContext
    {
        internal Type EntityType { get; }
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }
        internal List<IncludeInfo> Includes { get; private set; } = new();

        internal BHEntityContext(Type entityType, bool useActivator, string tableName, List<string> tableColumns, string propNames, string propParams, string updateParams)
        {
            EntityType = entityType;
            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
        }
    }

    /// <summary>
    /// Contains the required settings of the Entity so the
    /// Data Provider's Extension methods can use it.
    /// </summary>
    /// <typeparam name="T">BlackHoleEntity</typeparam>
    public class BHEntityContext<T>
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }
        internal string ConnectionString { get; }

        internal List<IncludeInfo> Includes { get; private set; } = new();

        internal BHEntityContext(bool useActivator, string tableName, List<string> tableColumns, string propNames, string propParams, string updateParams, string databaseName)
        {

            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
            ConnectionString = databaseName.BuildConnectionString();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public class BHEntityContext<T, G> where T : BHEntity  where G : BHEntity
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }
        internal string ConnectionString { get; }

        internal List<IncludeInfo> Includes { get; private set; } = new();

        internal BHEntityContext(bool useActivator, string tableName, List<string> tableColumns, string propNames, string propParams, string updateParams, string databaseName)
        {

            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
            ConnectionString = databaseName.BuildConnectionString();
        }
    }
}
