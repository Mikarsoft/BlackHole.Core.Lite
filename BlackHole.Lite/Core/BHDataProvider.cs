using BlackHole.CoreSupport;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// The main Blackhole Data Provider
    /// </summary>
    public static class BHDataProvider
    {
        internal static List<BHEntityContext> EntitiesContext = new();
        internal static List<StoredView> StoredViews = new();

        /// <summary>
        /// Run a custom Sql Command using this property
        /// </summary>
        public static BHConnection Command = new();

        internal static void SetExecutionProvider()
        {
            Command = new BHConnection();
        }

        /// <summary>
        /// Selects the correct EntityContext that will be used by the Data Provider
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <returns>Entity Context</returns>
        public static BHEntityContext<T> For<T>() where T : BlackHoleEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>(string.Empty);
        }

        /// <summary>
        /// Selects the correct EntityContext that will be used by the Data Provider
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <returns>Entity Context</returns>
        public static BHEntityContext<T> For<T>(string databaseName) where T : BlackHoleEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>(databaseName.BuildConnectionString());
        }

        /// <summary>
        /// Execute a view that has been stored under the specified BlackHoleDto
        /// </summary>
        /// <typeparam name="Dto">BlackHoleDto</typeparam>
        /// <returns>List of Dto</returns>
        public static List<Dto> ExecuteView<Dto>() where Dto : BlackHoleDto
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (storedV != null)
            {
                return storedV.ExecuteView<Dto>();
            }

            return new();
        }

        /// <summary>
        /// Execute a view that has been stored under the specified BlackHoleDto
        /// </summary>
        /// <typeparam name="Dto">BlackHoleDto</typeparam>
        /// <param name="bhTransaction">Transaction Object</param>
        /// <returns>List of Dto</returns>
        public static List<Dto> ExecuteView<Dto>(BHTransaction bhTransaction) where Dto : BlackHoleDto
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (storedV != null)
            {
                return storedV.ExecuteView<Dto>(bhTransaction);
            }

            return new();
        }

        internal static BHEntityContext<T> MapEntity<T>(this BHEntityContext context, string connectionString) where T : BlackHoleEntity
        {
            return new(context.WithActivator, context.ThisTable, context.Columns, context.PropertyNames, context.PropertyParams, context.UpdateParams, connectionString);
        }

        internal static void AddStoredView<Dto>(StoredView addView)
        {
            StoredView? storedV = StoredViews.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (storedV != null)
            {
                StoredViews.Remove(storedV);
            }

            StoredViews.Add(addView);
        }

        /// <summary>
        /// Starts an Inner Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <returns>PrejoinData</returns>
        public static PreJoinsData<T, TOther> InnerJoin<T, TOther>()
            where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                JoinType = "inner"
            };
        }

        /// <summary>
        /// Starts a Full Outer Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <returns>PrejoinData</returns>
        public static PreJoinsData<T, TOther> OuterJoin<T, TOther>()
            where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                JoinType = "full outer"
            };
        }

        /// <summary>
        /// Starts a Left Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <returns>PrejoinData</returns>
        public static PreJoinsData<T, TOther> LeftJoin<T, TOther>()
            where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                JoinType = "left"
            };
        }

        /// <summary>
        /// Starts a Right Join Between two Entities which can 
        /// extend with additional joins.
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <typeparam name="TOther">Second BlackHoleEntity</typeparam>
        /// <returns>PrejoinData</returns>
        public static PreJoinsData<T, TOther> RightJoin<T, TOther>()
            where T : BlackHoleEntity where TOther : BlackHoleEntity
        {
            return new PreJoinsData<T, TOther>()
            {
                JoinType = "right"
            };
        }
    }
}
