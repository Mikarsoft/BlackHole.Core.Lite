using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// A simple data provider that executes the commands only once, when a database is created.
    /// </summary>
    public class BHDataInitializer
    {
        internal List<BHEntityContext> EntitiesContext { get; }
        /// <summary>
        /// Run a custom Sql Command using this property
        /// </summary>
        public BHConnection Command { get; }

        /// <summary>
        /// A simple data provider that executes the commands only once, when a database is created.
        /// </summary>
        public BHDataInitializer()
        {
            EntitiesContext = BHDataProvider.EntitiesContext;
            Command = BHDataProvider.Command;
        }

        /// <summary>
        /// Selects the correct EntityContext that will be used by the Data Provider
        /// </summary>
        /// <typeparam name="T">BlackHoleEntity</typeparam>
        /// <returns>Entity Context</returns>
        public BHEntityContext<T> For<T>() where T : BlackHoleEntity
        {
            return EntitiesContext.First(x => x.EntityType == typeof(T)).MapEntity<T>();
        }
    }
}
