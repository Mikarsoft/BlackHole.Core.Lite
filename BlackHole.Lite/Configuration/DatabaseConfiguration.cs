using BlackHole.Core;
using BlackHole.Logger;
using BlackHole.Statics;
using SQLitePCL;

namespace BlackHole.Configuration
{
    internal static class DatabaseConfiguration
    {
        internal static void LogsSettings(string LogsPath)
        {
            DatabaseStatics.DataPath = LogsPath;
            LoggerService.DeleteOldLogs();
            LoggerService.SetUpLogger();
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

        internal static void ScanConnectionString(string connectionString)
        {
            ScanLiteString(connectionString, 0);
        }

        internal static void ScanConnectionStrings(List<string> databases , string dataPath)
        {
            DatabaseStatics.ConnectionStrings = new string[databases.Count];
            for(int i = 0; i < databases.Count; i++)
            {
                ScanLiteString(Path.Combine(dataPath, $"{databases[i]}.db3"), i);
            }
        }

        private static void ScanLiteString(string connectionString, int index)
        {
            try
            {
                string[] pathSplit = connectionString.Split('\\');
                string[] nameOnly = pathSplit[pathSplit.Length - 1].Split('.');
                DatabaseStatics.DatabaseName = nameOnly[0];
            }
            catch
            {
                DatabaseStatics.DatabaseName = connectionString;
            }
            Batteries_V2.Init();
            raw.SetProvider(new SQLite3Provider_e_sqlite3());
            DatabaseStatics.ConnectionStrings[index] = $"Data Source={connectionString};";
            BHDataProvider.SetExecutionProvider();
        }
    }
}