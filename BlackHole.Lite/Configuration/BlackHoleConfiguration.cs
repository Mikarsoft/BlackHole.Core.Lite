using BlackHole.Internal;
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
        public static void SuperNova(string dataPath)
        {
            if (DatabaseConfiguration.IsAutoUpdateOn())
            {
                Assembly GameAssembly = Assembly.GetCallingAssembly();
                string tempPath = Path.Combine(dataPath, "BlackHoleData");
                ScanConnectionString("BhDatabase", tempPath);
                DataPathAndLogs(tempPath);
                BuildDatabase(GameAssembly, "BhDatabase");
            }
        }

        /// <summary>
        /// Creates or Updates the Database Automatically into the specified Path
        /// and also registers all the Entities of the Project to the BHDataProvider.
        /// </summary>
        /// <param name="dataPath">The Path of the Database</param>
        public static void SuperNova()
        {
            if (DatabaseConfiguration.IsAutoUpdateOn())
            {
                Assembly GameAssembly = Assembly.GetCallingAssembly();
                string tempPath = Path.Combine(GameAssembly.Location, "..", "BlackHoleData");
                ScanConnectionString("BhDatabase", tempPath);
                DataPathAndLogs(tempPath);
                BuildDatabase(GameAssembly, "BhDatabase");
            }
        }

        /// <summary>
        /// Creates or Updates the Database Automatically into the specified Path
        /// and also registers all the Entities of the Project to the BHDataProvider.
        /// </summary>
        /// <param name="dataPath">The Path of the Database</param>
        public static void SuperNova(Action<List<string>> multipleDatabases)
        {
            if (DatabaseConfiguration.IsAutoUpdateOn())
            {
                Assembly GameAssembly = Assembly.GetCallingAssembly();
                string tempPath = Path.Combine(GameAssembly.Location, "..", "BlackHoleData");
                List<string> databases = new();
                multipleDatabases.Invoke(databases);
                ScanConnectionString("BhDatabase", tempPath);
                DataPathAndLogs(tempPath);
                BuildDatabase(GameAssembly, databases[0]);
            }
        }

        /// <summary>
        /// Creates or Updates the Database Automatically into the specified Path
        /// and also registers all the Entities of the Project to the BHDataProvider.
        /// </summary>
        /// <param name="dataPath">The Path of the Database</param>
        /// <param name="databaseName">The Name of the Database</param>
        public static void SuperNova(string dataPath, string databaseName)
        {
            if (DatabaseConfiguration.IsAutoUpdateOn())
            {
                Assembly GameAssembly = Assembly.GetCallingAssembly();
                string tempPath = Path.Combine(dataPath, "BlackHoleData");
                ScanConnectionString(databaseName, tempPath);
                DataPathAndLogs(tempPath);
                BuildDatabase(GameAssembly, databaseName);
            }
        }

        /// <summary>
        /// Creates or Updates the Database Automatically into the specified Path
        /// and also registers all the Entities of the Project to the BHDataProvider.
        /// </summary>
        /// <param name="dataPath">The Path of the Database</param>
        /// <param name="databaseName">The Name of the Database</param>
        public static void SuperNova(string dataPath, Action<List<string>> multipleDatabases)
        {
            if (DatabaseConfiguration.IsAutoUpdateOn())
            {
                Assembly GameAssembly = Assembly.GetCallingAssembly();
                string tempPath = Path.Combine(dataPath, "BlackHoleData");
                List<string> databases = new();
                multipleDatabases.Invoke(databases);
                ScanConnectionStrings(databases, tempPath);
                DataPathAndLogs(tempPath);
                BuildDatabase(GameAssembly, databases[0]);
            }
        }

        private static void BuildDatabase(Assembly callingAssembly, string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();

            if (databaseBuilder.CreateDatabase(databaseName))
            {
                CreateOrUpdateTables(callingAssembly, databaseBuilder);
            }
            else
            {
                throw new Exception("The Path to the database is inaccessible...");
            }
        }

        private static void CreateOrUpdateTables(Assembly callingAssembly, BHDatabaseBuilder databaseBuilder)
        {
            BHTableBuilder tableBuilder = new();
            BHNamespaceSelector namespaceSelector = new();
            BHInitialDataBuilder dataBuilder = new();
            tableBuilder.BuildMultipleTables(namespaceSelector.GetAllBHEntities(callingAssembly));
            if (databaseBuilder.IsCreatedFirstTime())
            {
                dataBuilder.InsertDefaultData(namespaceSelector.GetInitialData(callingAssembly));
            }
            dataBuilder.StoreDefaultViews(namespaceSelector.GetInitialViews(callingAssembly));
            tableBuilder.CleanupConstraints();
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
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static bool DropDatabase(string databaseName)
        {
            BHDatabaseBuilder databaseBuilder = new();
            return databaseBuilder.DropDatabase(databaseName);
        }

        private static void ScanConnectionString(string ConnectionString, string DataPath)
        {
            DatabaseConfiguration.ScanConnectionString(Path.Combine(DataPath, $"{ConnectionString}.db3"));
        }

        private static void ScanConnectionStrings(List<string> ConnectionStrings, string DataPath)
        {
            DatabaseConfiguration.ScanConnectionStrings(ConnectionStrings, DataPath);
        }

        private static void DataPathAndLogs(string DataPath)
        {
            DatabaseConfiguration.LogsSettings(DataPath);
        }
    }
}
