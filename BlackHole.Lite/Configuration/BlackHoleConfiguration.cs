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
        /// Creates or Updates the Database Automatically into the specified Path
        /// and also registers all the Entities of the Project to the BHDataProvider.
        /// </summary>
        /// <param name="dataPath">The Path of the Database</param>
        public static void SuperNova(Action<BlackHoleLiteSettings> settings)
        {
            BlackHoleLiteSettings InsideSettings = new();

            settings.Invoke(InsideSettings);

            string dataPath;

            if (string.IsNullOrEmpty(InsideSettings.DataPath.DataPath))
            {
                dataPath = AppDomain.CurrentDomain.BaseDirectory;
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
        /// Checks the database's condition
        /// </summary>
        /// <returns>Database is Up</returns>
        public static bool TestDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists(string.Empty);
        }

        /// <summary>
        /// Closes all connections and drops the database.
        /// </summary>
        /// <returns>Success</returns>
        public static bool DropDatabase()
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(string.Empty);
        }

        /// <summary>
        /// Checks the database's condition
        /// </summary>
        /// <returns>Database is Up</returns>
        public static bool TestDatabase(string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DoesDbExists(databaseName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool DropDatabase(string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(databaseName);
        }
    }
}
