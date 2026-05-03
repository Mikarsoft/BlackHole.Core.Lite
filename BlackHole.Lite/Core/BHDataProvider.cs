using BlackHole.CoreSupport;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// The main static facade for the BlackHole ORM. Provides entry points for entity queries,
    /// transactions, joins, and raw SQL execution.
    /// </summary>
    /// <remarks>
    /// <see cref="For{T}()"/> is the primary entry point for CRUD operations. All operations
    /// target the default database by default; use overloads with databaseName to target
    /// a specific registered database. Entity context operations support eager-loading via
    /// <see cref="ExecutionMethods.Include{T,G}"/> and <see cref="ExecutionMethods.ThenInclude{T,G,H}"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic insert and query
    /// var userId = BHDataProvider.For&lt;User&gt;().InsertEntry(new User { Name = "Alice" });
    /// var user = BHDataProvider.For&lt;User&gt;().GetEntryById(userId);
    /// </code>
    /// </example>
    public static class BHDataProvider
    {
        internal static List<BHEntityContext> EntitiesContext = new();

        internal static List<StoredView> StoredViews = new();

        internal static Dictionary<string, string> FkMap = new Dictionary<string, string>();
        internal static Dictionary<string, string> FkReverseMap = new Dictionary<string, string>();

        internal static string DefaultDbName {  get; set; } = string.Empty;

        /// <summary>
        /// Returns a connection object for executing raw SQL commands against the default database.
        /// </summary>
        /// <remarks>
        /// The connection uses the default registered database. For multiple databases,
        /// use <see cref="GetConnection(string)"/> with the desired database name.
        /// </remarks>
        /// <returns>A <see cref="BHConnection"/> object for raw SQL execution.</returns>
        /// <example>
        /// <code>
        /// var conn = BHDataProvider.GetConnection();
        /// var result = conn.QueryFirst&lt;int&gt;("select count(*) from Users");
        /// </code>
        /// </example>
        public static BHConnection GetConnection()
        {
            return new BHConnection();
        }

        /// <summary>
        /// Returns a connection object for executing raw SQL commands against a specific database.
        /// </summary>
        /// <param name="databaseName">Name of the registered database to connect to.</param>
        /// <remarks>
        /// Use this overload when your application uses multiple databases. The connection
        /// automatically targets the specified database file.
        /// </remarks>
        /// <returns>A <see cref="BHConnection"/> object for raw SQL execution against the specified database.</returns>
        public static BHConnection GetConnection(string databaseName)
        {
            return new BHConnection(databaseName);
        }

        /// <summary>
        /// Gets the entity context for the specified entity type, using the default database.
        /// </summary>
        /// <typeparam name="T">An entity class deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// This is the primary entry point for all CRUD operations. The returned
        /// <see cref="BHEntityContext{T}"/> exposes methods like <see cref="ExecutionMethods.GetAllEntries{T}"/>,
        /// <see cref="ExecutionMethods.InsertEntry{T}"/>, <see cref="ExecutionMethods.UpdateEntryById{T}"/>,
        /// and <see cref="ExecutionMethods.DeleteEntryById{T}"/>. When the entity has <c>[UseActivator]</c>,
        /// soft-delete semantics apply: <see cref="ExecutionMethods.DeleteEntryById{T}"/> sets Inactive=1
        /// rather than deleting, and <see cref="ExecutionMethods.GetAllEntries{T}"/> automatically
        /// filters out inactive rows.
        /// </remarks>
        /// <returns>An entity context for executing queries, inserts, updates, and deletes on <typeparamref name="T"/>.</returns>
        /// <example>
        /// <code>
        /// var context = BHDataProvider.For&lt;User&gt;();
        /// var users = context.GetAllEntries();
        /// var user = context.GetEntryById(5);
        /// var newId = context.InsertEntry(new User { Name = "Bob" });
        /// context.UpdateEntryById(new User { Id = 5, Name = "Bob Updated" });
        /// context.DeleteEntryById(5);
        /// </code>
        /// </example>
        public static BHEntityContext<T> For<T>() where T : BHEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>(DefaultDbName.BuildConnectionString());
        }

        /// <summary>
        /// Gets the entity context for the specified entity type, targeting a specific database.
        /// </summary>
        /// <typeparam name="T">An entity class deriving from <see cref="BHEntity"/>.</typeparam>
        /// <param name="databaseName">The name of the registered database to query.</param>
        /// <remarks>
        /// Use this overload for multi-database scenarios. The returned context operates
        /// on the specified database; all CRUD methods will target that database.
        /// </remarks>
        /// <returns>An entity context for executing queries on the specified database.</returns>
        public static BHEntityContext<T> For<T>(string databaseName) where T : BHEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>(databaseName);
        }

        /// <summary>
        /// Executes a stored join view against the default database, returning results mapped to the specified DTO.
        /// </summary>
        /// <typeparam name="Dto">A class deriving from <see cref="BlackHoleDto"/>, representing the join result shape.</typeparam>
        /// <remarks>
        /// Views are created once at startup via <see cref="IBHInitialViews.DefaultViews"/> and
        /// <see cref="ExecutionMethods.StoreAsView{Dto}"/>. When executed, the materialized query
        /// is returned as a list of DTOs. If no view is registered for this DTO, an empty list is returned.
        /// </remarks>
        /// <returns>A list of DTO instances from the stored view, or empty if not registered.</returns>
        public static List<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto) && x.DatabaseName == DefaultDbName);

            if (storedV != null)
            {
                return storedV.ExecuteView<Dto>();
            }

            return new();
        }

        /// <summary>
        /// Executes a stored join view against a specific database, returning results mapped to the specified DTO.
        /// </summary>
        /// <typeparam name="Dto">A class deriving from <see cref="BlackHoleDto"/>, representing the join result shape.</typeparam>
        /// <param name="databaseName">The name of the registered database to execute the view on.</param>
        /// <returns>A list of DTO instances from the stored view on the specified database, or empty if not registered.</returns>
        public static List<Dto> ExecuteView<Dto>(string databaseName) where Dto : BlackHoleDto
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto) && x.DatabaseName == databaseName);

            if (storedV != null)
            {
                return storedV.ExecuteView<Dto>();
            }

            return new();
        }

        /// <summary>
        /// Executes a stored join view within a transaction, returning results mapped to the specified DTO.
        /// </summary>
        /// <typeparam name="Dto">A class deriving from <see cref="BlackHoleDto"/>, representing the join result shape.</typeparam>
        /// <param name="bhTransaction">The <see cref="BHTransaction"/> to execute the view within.</param>
        /// <remarks>
        /// The view executes on the same database as the transaction. This is useful when
        /// you want the view result to be consistent with concurrent transactional changes.
        /// </remarks>
        /// <returns>A list of DTO instances from the stored view, or empty if not registered.</returns>
        public static List<Dto> ExecuteView<Dto>(BHTransaction bhTransaction) where Dto : BlackHoleDto
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto) && x.DatabaseName == bhTransaction.DBName);

            if (storedV != null)
            {
                return storedV.ExecuteView<Dto>(bhTransaction);
            }

            return new();
        }

        internal static BHEntityContext<T> MapEntity<T>(this BHEntityContext context, string connectionString) where T : BHEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames,
                context.PropertyParams, context.UpdateParams, connectionString);
        }

        internal static BHEntityContext<T, G> MapEntity<T, G>(this BHEntityContext<T> context)
            where T : BHEntity where G : BHEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames,
                context.PropertyParams, context.UpdateParams, context.Includes, context.ConnectionString);
        }

        internal static BHEntityContext<T, H> MapEntity<T, G, H>(this BHEntityContext<T, G> context)
            where T : BHEntity where G : BHEntity where H : BHEntity
        {
            return new (context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames,
                context.PropertyParams, context.UpdateParams, context.Includes, context.ConnectionString);
        }

        internal static BHTransactEntityContext<T> MapEntityTransact<T>(this BHEntityContext<T> context, 
            BlackHoleTransaction transaction) where T : BHEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns,
                context.PropertyNames, context.PropertyParams, context.UpdateParams, transaction);
        }

        internal static BHTransactEntityContext<T, G> MapEntityTransact<T, G>(this BHTransactEntityContext<T> context)
            where T : BHEntity where G : BHEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames,
                context.PropertyParams, context.UpdateParams, context.Includes, context._transatcion);
        }

        internal static BHTransactEntityContext<T, H> MapEntityTransact<T, G, H>(this BHTransactEntityContext<T, G> context)
            where T : BHEntity where G : BHEntity where H : BHEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames,
                context.PropertyParams, context.UpdateParams, context.Includes, context._transatcion);
        }

        internal static void AddStoredView<Dto>(StoredView addView, string databaseName)
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto) && x.DatabaseName == databaseName);

            if (storedV != null)
            {
                StoredViews.Remove(storedV);
            }

            StoredViews.Add(addView);
        }

        /// <summary>
        /// Initiates an inner join between two entities, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The first entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The second entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// The returned <see cref="PreJoinsData{T,TOther}"/> must be configured with <c>.On(...)</c>
        /// to specify the join key, then optionally filtered with <c>.Where()</c> and executed
        /// with <c>.ExecuteQuery&lt;Dto&gt;()</c> or stored with <c>.StoreAsView&lt;Dto&gt;()</c>.
        /// </remarks>
        /// <returns>A pre-join data object for specifying join conditions.</returns>
        /// <example>
        /// <code>
        /// var results = BHDataProvider.InnerJoin&lt;User, Order&gt;()
        ///     .On(u => u.Id, o => o.UserId)
        ///     .ExecuteQuery&lt;UserOrderDto&gt;();
        /// </code>
        /// </example>
        public static PreJoinsData<T, TOther> InnerJoin<T, TOther>()
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = DefaultDbName,
                JoinType = "inner"
            };
        }

        /// <summary>
        /// Initiates a full outer join between two entities, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The first entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The second entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// A full outer join returns all rows from both entities where the join condition matches,
        /// plus unmatched rows from both sides. Configure with <c>.On(...)</c>, filter as needed,
        /// and execute or store.
        /// </remarks>
        /// <returns>A pre-join data object for specifying join conditions.</returns>
        public static PreJoinsData<T, TOther> OuterJoin<T, TOther>()
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = DefaultDbName,
                JoinType = "full outer"
            };
        }

        /// <summary>
        /// Initiates a left join between two entities, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The left entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The right entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// A left join returns all rows from the left entity and matching rows from the right.
        /// Unmatched rows from the right entity are excluded.
        /// </remarks>
        /// <returns>A pre-join data object for specifying join conditions.</returns>
        public static PreJoinsData<T, TOther> LeftJoin<T, TOther>()
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = DefaultDbName,
                JoinType = "left"
            };
        }

        /// <summary>
        /// Initiates a right join between two entities, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The left entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The right entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// A right join returns all rows from the right entity and matching rows from the left.
        /// Unmatched rows from the left entity are excluded.
        /// </remarks>
        /// <returns>A pre-join data object for specifying join conditions.</returns>
        public static PreJoinsData<T, TOther> RightJoin<T, TOther>()
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = DefaultDbName,
                JoinType = "right"
            };
        }


        /// <summary>
        /// Initiates an inner join between two entities on a specific database, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The first entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The second entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <param name="databaseName">The name of the registered database to execute the join on.</param>
        /// <returns>A pre-join data object for specifying join conditions on the specified database.</returns>
        public static PreJoinsData<T, TOther> InnerJoin<T, TOther>(string databaseName)
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = databaseName,
                JoinType = "inner"
            };
        }

        /// <summary>
        /// Initiates a full outer join between two entities on a specific database, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The first entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The second entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <param name="databaseName">The name of the registered database to execute the join on.</param>
        /// <returns>A pre-join data object for specifying join conditions on the specified database.</returns>
        public static PreJoinsData<T, TOther> OuterJoin<T, TOther>(string databaseName)
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = databaseName,
                JoinType = "full outer"
            };
        }

        /// <summary>
        /// Initiates a left join between two entities on a specific database, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The left entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The right entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <param name="databaseName">The name of the registered database to execute the join on.</param>
        /// <returns>A pre-join data object for specifying join conditions on the specified database.</returns>
        public static PreJoinsData<T, TOther> LeftJoin<T, TOther>(string databaseName)
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = databaseName,
                JoinType = "left"
            };
        }

        /// <summary>
        /// Initiates a right join between two entities on a specific database, returning a pre-join data object for configuration.
        /// </summary>
        /// <typeparam name="T">The left entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <typeparam name="TOther">The right entity class, deriving from <see cref="BHEntity"/>.</typeparam>
        /// <param name="databaseName">The name of the registered database to execute the join on.</param>
        /// <returns>A pre-join data object for specifying join conditions on the specified database.</returns>
        public static PreJoinsData<T, TOther> RightJoin<T, TOther>(string databaseName)
            where T : BHEntity where TOther : BHEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                DatabaseName = databaseName,
                JoinType = "right"
            };
        }
    }
}
