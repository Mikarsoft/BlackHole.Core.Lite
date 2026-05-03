using BlackHole.Internal;
using BlackHole.Lite.Configuration;
using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// Has methods for configuring the database Path and Name.
    /// It can also Drop the database or check its condition.
    /// </summary>
    public static class BlackHoleConfiguration
    {
        /// <summary>
        /// The main entry point for bootstrapping BlackHole.Lite. Scans the calling assembly for all <c>BHEntity</c> subclasses,
        /// creates or updates SQLite databases to match their schemas, and registers them with the ORM.
        /// This method must be called exactly once at application startup before any database operations.
        /// </summary>
        /// <param name="settings">A configuration action that receives a <see cref="BlackHoleLiteSettings"/> instance.
        /// Use this to register one or more databases via <see cref="BlackHoleLiteSettings.AddDatabase(string)"/> calls,
        /// optionally customizing the data folder path.</param>
        /// <remarks>
        /// SuperNova performs the following operations:
        /// - Scans the calling assembly for classes that inherit from BHEntity.
        /// - Creates SQLite database files in the configured data folder (default: <c>&lt;AppBase&gt;/BlackHoleData</c>).
        /// - Creates or updates tables to match entity properties and relationships.
        /// - Runs any initial data insertion logic (IInitialData implementations).
        /// - Stores any defined database views (IInitialViews implementations).
        ///
        /// If a namespace filter is set on a database, only entities in that namespace are included for that database.
        /// Duplicate database names will throw an exception.
        /// </remarks>
        /// <exception cref="Exception">Thrown if duplicate database names are detected or if the data path is inaccessible.</exception>
        /// <example>
        /// <code>
        /// // Typical startup configuration
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabase("MyApplicationDb")
        ///             .SetDataPath(@"C:\MyApp\Data");
        /// });
        ///
        /// // Multi-database configuration
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabase("UsersDb", "MyApp.Domain.Users");
        ///     settings.AddDatabase("ProductsDb", "MyApp.Domain.Products")
        ///             .SetDataPath(@"D:\DatabaseFiles");
        /// });
        /// </code>
        /// </example>
        public static void SuperNova(Action<BlackHoleLiteSettings> settings)
        {
            BlackHoleLiteSettings InsideSettings = new();

            settings.Invoke(InsideSettings);

            string dataPath;

            if (string.IsNullOrEmpty(InsideSettings.DataPath.DataPath))
            {
                dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BlackHoleData");
            }
            else
            {
                dataPath = InsideSettings.DataPath.DataPath;
            }

            dataPath.SetDataPathAndLogging();

            List<string> databaseNames = InsideSettings.DbSettings.Select(x => x.DatabaseName).ToList();

            if(databaseNames.Count != databaseNames.Distinct().Count())
            {
                throw new Exception("Error. There are multiple databases with the same name. Please remove duplicate database names.");
            }

            InsideSettings.DbSettings.CreateConnectionStrings();

            Assembly GameAssembly = Assembly.GetCallingAssembly();

            InsideSettings.BuildDatabases(GameAssembly);
        }

        private static void BuildDatabases(this BlackHoleLiteSettings settings , Assembly callingAssembly)
        {
            BHDatabaseBuilder databaseBuilder = new();
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();

            foreach (DatabaseSettings dbSettings in settings.DbSettings)
            {
                if (databaseBuilder.CreateDatabase(dbSettings.DatabaseName))
                {
                    tableBuilder.SwitchConnectionString(dbSettings.ConnectionString);

                    if (string.IsNullOrEmpty(dbSettings.UsingNamespace))
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly));

                        if (databaseBuilder.IsCreatedFirstTime())
                        {
                            dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly), dbSettings.DatabaseName);
                        }

                        dataBuilder.StoreDefaultViews(namespaceSelector.GetInitialViews(callingAssembly), dbSettings.DatabaseName);
                    }
                    else
                    {
                        tableBuilder.BuildMultipleTables(namespaceSelector.GetBHEntitiesInNamespace(callingAssembly, dbSettings.UsingNamespace));

                        if (databaseBuilder.IsCreatedFirstTime())
                        {
                            dataBuilder.InsertDefaultData(namespaceSelector.GetInitialDataInNamespace(callingAssembly, dbSettings.UsingNamespace), dbSettings.DatabaseName);
                        }

                        dataBuilder.StoreDefaultViews(namespaceSelector.GetInitialViewsInNamespace(callingAssembly, dbSettings.UsingNamespace), dbSettings.DatabaseName);
                    }

                    tableBuilder.CleanupConstraints();
                }
                else
                {
                    throw new Exception("The Path to the database is inaccessible...");
                }
            }
        }

        /// <summary>
        /// Tests connectivity to the default database by attempting to verify its existence.
        /// This is useful for health checks or diagnostics after SuperNova initialization.
        /// </summary>
        /// <returns>True if the default database exists and is accessible; false otherwise.</returns>
        /// <remarks>
        /// This method tests only the default (first registered) database. If you have registered multiple databases,
        /// use the overload <see cref="TestDatabase(string)"/> to test a specific database by name.
        /// </remarks>
        public static bool TestDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists(string.Empty);
        }

        /// <summary>
        /// Closes all connections and permanently deletes the default database file.
        /// This operation is irreversible and will lose all data.
        /// </summary>
        /// <returns>True if the database was successfully dropped; false if the operation failed or the database did not exist.</returns>
        /// <remarks>
        /// This method only affects the default (first registered) database. To drop a specific named database,
        /// use the overload <see cref="DropDatabase(string)"/>.
        /// All active connections must be closed before the file can be deleted; this method handles that automatically.
        /// </remarks>
        public static bool DropDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(string.Empty);
        }

        /// <summary>
        /// Tests connectivity to a specific named database by attempting to verify its existence.
        /// This is useful for health checks or diagnostics on individual databases in a multi-database setup.
        /// </summary>
        /// <param name="databaseName">The name of the database to test (as registered via SuperNova).
        /// Must match exactly the database name passed to <see cref="BlackHoleLiteSettings.AddDatabase(string)"/>.</param>
        /// <returns>True if the database exists and is accessible; false otherwise.</returns>
        public static bool TestDatabase(string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists(databaseName);
        }

        /// <summary>
        /// Closes all connections and permanently deletes a specific named database file.
        /// This operation is irreversible and will lose all data in that database.
        /// </summary>
        /// <param name="databaseName">The name of the database to drop (as registered via SuperNova).
        /// Must match exactly the database name passed to <see cref="BlackHoleLiteSettings.AddDatabase(string)"/>.</param>
        /// <returns>True if the database was successfully dropped; false if the operation failed or the database did not exist.</returns>
        /// <remarks>
        /// All active connections to the specified database must be closed before the file can be deleted;
        /// this method handles that automatically. Other databases in a multi-database setup remain unaffected.
        /// </remarks>
        public static bool DropDatabase(string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(databaseName);
        }
    }
}
