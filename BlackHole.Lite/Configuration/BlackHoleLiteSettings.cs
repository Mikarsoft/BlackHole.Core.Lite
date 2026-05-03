
namespace BlackHole.Lite.Configuration
{
    /// <summary>
    /// Configures one or more SQLite databases for the BlackHole.Lite ORM.
    /// This class is used internally by <see cref="BlackHoleConfiguration.SuperNova(Action{BlackHoleLiteSettings})"/>
    /// to register databases and specify data folder paths. Users invoke it indirectly by passing a configuration
    /// lambda to SuperNova.
    /// </summary>
    public class BlackHoleLiteSettings
    {
        internal DataPathSettings DataPath { get; set; } = new();
        internal List<DatabaseSettings> DbSettings { get; set; } = new();


        /// <summary>
        /// Registers a single SQLite database with the given name. The database will be created or updated
        /// automatically to match all <c>BHEntity</c> subclasses found in the calling assembly.
        /// </summary>
        /// <param name="databaseName">The name of the database file (without .db extension).
        /// The file will be created in the data folder specified by <see cref="DataPathSettings.SetDataPath(string)"/>,
        /// or in the default folder <c>&lt;AppBase&gt;/BlackHoleData</c> if not customized.</param>
        /// <returns>A <see cref="DataPathSettings"/> instance that allows chaining to customize the data folder path.</returns>
        /// <remarks>
        /// If no namespace filter is set, all BHEntity classes in the assembly are mapped to this database.
        /// To restrict which entities map to this database, use the overload that accepts <c>useNamespace</c>.
        /// Call this method multiple times to register multiple databases.
        /// </remarks>
        /// <example>
        /// <code>
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabase("MyAppDb");
        /// });
        /// </code>
        /// </example>
        public DataPathSettings AddDatabase(string databaseName)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, string.Empty));
            return DataPath;
        }

        /// <summary>
        /// Registers a single SQLite database with the given name and restricts entity mapping to a specific namespace.
        /// Only <c>BHEntity</c> subclasses in the specified namespace will be created/updated in this database.
        /// </summary>
        /// <param name="databaseName">The name of the database file (without .db extension).
        /// The file will be created in the data folder specified by <see cref="DataPathSettings.SetDataPath(string)"/>,
        /// or in the default folder <c>&lt;AppBase&gt;/BlackHoleData</c> if not customized.</param>
        /// <param name="useNamespace">The fully-qualified namespace (e.g., <c>"MyApp.Domain.Models"</c>)
        /// that restricts which entities map to this database. Only entity classes in this namespace
        /// (or nested namespaces) will be included. Leave empty or null if you want all entities to use this database.</param>
        /// <returns>A <see cref="DataPathSettings"/> instance that allows chaining to customize the data folder path.</returns>
        /// <remarks>
        /// This overload enables multi-database architectures where different entity types are stored in separate databases
        /// but managed by the same ORM instance. For example, you could have audit logs in one database and user data in another.
        /// </remarks>
        /// <example>
        /// <code>
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabase("UsersDb", "MyApp.Domain.Users");
        ///     settings.AddDatabase("OrdersDb", "MyApp.Domain.Orders");
        /// });
        /// </code>
        /// </example>
        public DataPathSettings AddDatabase(string databaseName, string useNamespace)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, useNamespace));
            return DataPath;
        }

        /// <summary>
        /// Registers multiple SQLite databases in a single batch operation. This is a convenience method
        /// that allows you to configure multiple databases fluently within a single call.
        /// </summary>
        /// <param name="multipleDatabase">An action that receives a list of actions, each configuring one database.
        /// Each action in the list is invoked with a fresh <see cref="DatabaseSettings"/> instance that you populate
        /// by calling <see cref="DatabaseSettings.AddDatabase(string)"/> or <see cref="DatabaseSettings.AddDatabase(string, string)"/>.</param>
        /// <returns>A <see cref="DataPathSettings"/> instance that allows chaining to customize the data folder path for all registered databases.</returns>
        /// <remarks>
        /// This method is useful when registering many databases that share the same data folder.
        /// The data path set on the returned <see cref="DataPathSettings"/> applies to all databases added via this call.
        /// </remarks>
        /// <example>
        /// <code>
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabases(dbs =>
        ///     {
        ///         dbs.Add(db => db.AddDatabase("UsersDb", "MyApp.Users"));
        ///         dbs.Add(db => db.AddDatabase("OrdersDb", "MyApp.Orders"));
        ///         dbs.Add(db => db.AddDatabase("ProductsDb", "MyApp.Products"));
        ///     }).SetDataPath("/custom/data/folder");
        /// });
        /// </code>
        /// </example>
        public DataPathSettings AddDatabases(Action<List<Action<DatabaseSettings>>> multipleDatabase)
        {
            List<Action<DatabaseSettings>> list = new();
            multipleDatabase.Invoke(list);

            foreach (Action<DatabaseSettings> settings in list)
            {
                DatabaseSettings dbSettings = new();
                settings.Invoke(dbSettings);
                DbSettings.Add(dbSettings);
            }

            return DataPath;
        }
    }
}
