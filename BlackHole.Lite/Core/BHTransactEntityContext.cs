using BlackHole.CoreSupport;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// Provides a fluent context for executing transactional queries, inserts, updates, and deletes on a single entity type.
    /// Returned by <see cref="BHTransaction.For{T}()"/>, all operations enlist in the parent transaction.
    /// </summary>
    /// <typeparam name="T">An entity class deriving from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// This context is the transactional sibling of <see cref="BHEntityContext{T}"/>. All CRUD operations
    /// execute within the scope of a <see cref="BHTransaction"/>, meaning they can be atomically committed
    /// or rolled back together. The transaction will auto-rollback on <c>Dispose()</c> if not explicitly committed.
    /// </remarks>
    public class BHTransactEntityContext<T> where T : BHEntity
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
    /// Represents a transactional entity context with a single level of eager-loaded navigation property.
    /// Returned by <see cref="ExecutionTransactMethods.Include{T,G}"/>, this allows chaining additional
    /// <c>ThenInclude()</c> or executing transactional queries with the included relationship materialized.
    /// </summary>
    /// <typeparam name="T">The root entity class, deriving from <see cref="BHEntity"/>.</typeparam>
    /// <typeparam name="G">The related entity class to eagerly load, deriving from <see cref="BHEntity"/>.</typeparam>
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
