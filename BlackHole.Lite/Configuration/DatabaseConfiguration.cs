using BlackHole.Core;
using BlackHole.CoreSupport;
using BlackHole.Lite.Configuration;
using BlackHole.Logger;
using BlackHole.Statics;
using SQLitePCL;

namespace BlackHole.Configuration
{
    internal static class DatabaseConfiguration
    {
        internal static void SetDataPathAndLogging(this string dataPath)
        {
            if (string.IsNullOrEmpty(DatabaseStatics.DataPath))
            {
                DatabaseStatics.DataPath = Path.Combine(dataPath,"BlackHoleData");
                LoggerService.DeleteOldLogs();
                LoggerService.SetUpLogger();
            }
        }

        internal static bool IsAutoUpdateOn()
        {
            if (DatabaseStatics.AutoUpdate)
            {
                DatabaseStatics.AutoUpdate = false;
                return true;
            }

            return false;
        }

        internal static void CreateConnectionStrings(this List<DatabaseSettings> databases)
        {
            if (databases.Any())
            {
                if (string.IsNullOrEmpty(DatabaseStatics.DefaultConnectionString))
                {
                    DatabaseStatics.DefaultConnectionString = databases[0].DatabaseName.BuildConnectionString();
                    DatabaseStatics.DefaultDatabaseName = databases[0].DatabaseName;
                    BHDataProvider.DefaultDbName = databases[0].DatabaseName;
                }

                foreach(DatabaseSettings database in databases)
                {
                    database.ConnectionString = database.DatabaseName.BuildConnectionString();
                }

                if (!DatabaseStatics.IsLiteLibInitialized)
                {
                    Batteries_V2.Init();
                    raw.SetProvider(new SQLite3Provider_e_sqlite3());
                    DatabaseStatics.IsLiteLibInitialized = true;
                }
            }
        }
    }
}