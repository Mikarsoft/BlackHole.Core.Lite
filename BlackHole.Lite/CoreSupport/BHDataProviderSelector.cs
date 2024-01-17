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
            string dataPath = Path.Combine(DatabaseStatics.DataPath, databaseName);
            return $"Data Source={dataPath}.db3;";
        }

        internal static SqliteDataProvider GetDataProvider()
        {
            return new SqliteDataProvider(DatabaseStatics.ConnectionStrings[0]);
        }

        internal static SqliteConnection GetConnection()
        {
            return new SqliteConnection(DatabaseStatics.ConnectionStrings[0]);
        }

        internal static bool CheckActivator(this Type entity)
        {
            return entity.GetCustomAttributes(true).Any(x => x.GetType() == typeof(UseActivator));
        }

        internal static SqliteExecutionProvider GetExecutionProvider()
        {
            return new SqliteExecutionProvider(DatabaseStatics.ConnectionStrings[0]);
        }
    }
}
