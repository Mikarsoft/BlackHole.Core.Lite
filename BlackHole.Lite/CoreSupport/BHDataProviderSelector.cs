using BlackHole.DataProviders;
using BlackHole.Entities;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;

namespace BlackHole.CoreSupport
{
    internal static class BHDataProviderSelector
    {
        internal static string BuildConnectionString(this string databaseName)
        {
            if(string.IsNullOrWhiteSpace(databaseName))
            {
                string datapathDefault = Path.Combine(DatabaseStatics.DataPath, DatabaseStatics.DefaultConnectionString);
                return $"Data Source={datapathDefault}.db3;";
            }

            string dataPath = Path.Combine(DatabaseStatics.DataPath, databaseName);
            return $"Data Source={dataPath}.db3;";
        }

        internal static string GetDefaultDbName()
        {
            return DatabaseStatics.DefaultDatabaseName;
        }

        internal static SqliteDataProvider GetDataProvider()
        {
            return new SqliteDataProvider();
        }

        internal static SqliteConnection GetConnection()
        {
            return new SqliteConnection(DatabaseStatics.DefaultConnectionString);
        }

        internal static SqliteConnection GetConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        internal static bool CheckActivator(this Type entity)
        {
            return entity.GetCustomAttributes(true).Any(x => x.GetType() == typeof(UseActivator));
        }

        internal static SqliteExecutionProvider GetExecutionProvider(string databaseName)
        {
            return new SqliteExecutionProvider(databaseName.BuildConnectionString());
        }

        internal static SqliteExecutionProvider GetExecutionProvider()
        {
            return new SqliteExecutionProvider(DatabaseStatics.DefaultConnectionString);
        }
    }
}
