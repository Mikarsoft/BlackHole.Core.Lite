

namespace BlackHole.Lite.Configuration
{
    /// <summary>
    /// Holds configuration for a single SQLite database, including its name and optional namespace filter.
    /// This class is typically populated through <see cref="BlackHoleLiteSettings.AddDatabases(Action{List{Action{DatabaseSettings}}})"/>
    /// when registering multiple databases at once.
    /// </summary>
    public class DatabaseSettings
    {
        internal string DatabaseName { get; set; }
        internal string UsingNamespace {  get; set; }

        internal string ConnectionString { get; set; } = string.Empty;

        internal DatabaseSettings()
        {
            DatabaseName = string.Empty;
            UsingNamespace = string.Empty;
        }

        internal DatabaseSettings(string databaseName, string selectedNamespace)
        {
            DatabaseName = databaseName;
            UsingNamespace = selectedNamespace;
        }

        /// <summary>
        /// Sets the name of this database without a namespace filter. All <c>BHEntity</c> subclasses
        /// in the calling assembly will map to this database.
        /// </summary>
        /// <param name="databaseName">The name of the database file (without .db extension).</param>
        /// <remarks>
        /// This method is typically used when called from a lambda passed to <see cref="BlackHoleLiteSettings.AddDatabases(Action{List{Action{DatabaseSettings}}})"/>.
        /// If you need to filter by namespace, use the overload that accepts both name and namespace instead.
        /// </remarks>
        public void AddDatabase(string databaseName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Sets the name of this database and restricts entity mapping to a specific namespace.
        /// Only <c>BHEntity</c> subclasses in the specified namespace will be created/updated in this database.
        /// </summary>
        /// <param name="databaseName">The name of the database file (without .db extension).</param>
        /// <param name="selectedNamespace">The fully-qualified namespace that restricts which entities map to this database.
        /// For example, <c>"MyApp.Domain.Orders"</c>. Leave empty if all entities should use this database.</param>
        /// <remarks>
        /// This method is typically used when called from a lambda passed to <see cref="BlackHoleLiteSettings.AddDatabases(Action{List{Action{DatabaseSettings}}})"/>.
        /// The namespace filter enables multi-database support where different entity groups are stored separately.
        /// </remarks>
        public void AddDatabase(string databaseName, string selectedNamespace)
        {
            DatabaseName = databaseName;
            UsingNamespace = selectedNamespace;
        }
    }
}
