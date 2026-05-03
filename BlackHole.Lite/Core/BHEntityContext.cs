using BlackHole.CoreSupport;
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

        internal BHEntityContext(Type entityType, bool useActivator, string tableName, List<string> tableColumns,
            string propNames, string propParams, string updateParams)
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
    /// Provides a fluent context for executing queries, inserts, updates, and deletes on a single entity type.
    /// Returned by <see cref="BHDataProvider.For{T}()"/>, this is the entry point for all CRUD operations.
    /// </summary>
    /// <typeparam name="T">An entity class deriving from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// Extension methods in <see cref="ExecutionMethods"/> are applied to this context to provide:
    /// - Query methods: <c>GetAllEntries()</c>, <c>GetEntryById()</c>, <c>GetEntryWhere()</c>, <c>Any()</c>, <c>Count()</c>
    /// - Modification methods: <c>InsertEntry()</c>, <c>UpdateEntryById()</c>, <c>DeleteEntryById()</c>
    /// - Navigation loading: <c>Include&lt;G&gt;()</c> to eagerly load related entities
    ///
    /// When an entity has <c>[UseActivator]</c>, delete operations set <c>Inactive=1</c> instead of deleting,
    /// and retrieval methods automatically filter out inactive rows unless using <c>GetAllInactiveEntries()</c>.
    /// </remarks>
    public class BHEntityContext<T>
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }
        internal string ConnectionString { get; }

        internal List<IncludePart> Includes { get; private set; } = new();

        internal BHEntityContext(bool useActivator, string tableName, List<string> tableColumns,
            string propNames, string propParams, string updateParams, string databaseName)
        {
            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
            ConnectionString = databaseName;
        }
    }


    /// <summary>
    /// Represents an entity context with a single level of eager-loaded navigation property.
    /// Returned by <see cref="ExecutionMethods.Include{T,G}"/>, this allows chaining additional
    /// <c>ThenInclude()</c> or executing queries with the included relationship materialized.
    /// </summary>
    /// <typeparam name="T">The root entity class, deriving from <see cref="BHEntity"/>.</typeparam>
    /// <typeparam name="G">The related entity class to eagerly load, deriving from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// This context is an intermediate step in an Include chain. The second type parameter <typeparamref name="G"/>
    /// represents the included relationship, and you can further chain with <c>ThenInclude&lt;H&gt;()</c>
    /// to load a relationship of <typeparamref name="G"/>.
    /// </remarks>
    public class BHEntityContext<T, G> where T : BHEntity  where G : BHEntity
    {
        internal bool WithActivator { get; }
        internal string ThisTable { get; }
        internal List<string> Columns { get; }
        internal string PropertyNames { get; }
        internal string PropertyParams { get; }
        internal string UpdateParams { get; }
        internal string ConnectionString { get; }
        internal List<IncludePart> Includes { get; private set; }

        internal BHEntityContext(bool useActivator, string tableName, List<string> tableColumns,
            string propNames, string propParams, string updateParams, List<IncludePart> include, string databaseName)
        {
            Includes = include;
            WithActivator = useActivator;
            ThisTable = tableName;
            Columns = tableColumns;
            PropertyNames = propNames;
            PropertyParams = propParams;
            UpdateParams = updateParams;
            ConnectionString = databaseName;
        }
    }
}
