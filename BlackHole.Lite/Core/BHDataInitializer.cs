using BlackHole.Entities;
using BlackHole.CoreSupport;
using BlackHole.Statics;

namespace BlackHole.Core
{
    /// <summary>
    /// A simple data provider that executes the commands only once, when a database is created.
    /// </summary>
    public class BHDataInitializer
    {
        internal List<BHEntityContext> EntitiesContext { get; }
        
        internal string ConnectionString { get; }
        internal string DatabaseName { get; }
        /// <summary>
        /// A simple data provider that executes the commands only once, when a database is created.
        /// </summary>
        internal BHDataInitializer(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                DatabaseName = DatabaseStatics.DefaultDatabaseName;
            }
            else
            {
                DatabaseName = databaseName;
            }

            EntitiesContext = BHDataProvider.EntitiesContext;
            ConnectionString = databaseName.BuildConnectionString();
        }

        /// <summary>
        /// Selects the correct EntityContext that will be used by the Data Provider
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <returns>Entity Context</returns>
        public BHEntityContext<T> For<T>() where T : BlackHoleEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>(DatabaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public BHConnection Command(string databaseName)
        {
            return new BHConnection(ConnectionString);
        }
    }
}
