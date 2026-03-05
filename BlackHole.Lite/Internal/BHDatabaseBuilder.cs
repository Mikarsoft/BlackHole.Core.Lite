using BlackHole.Core;
using BlackHole.CoreSupport;
using BlackHole.Lite.Logger;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;

namespace BlackHole.Internal
{
    internal class BHDatabaseBuilder
    {
        private readonly BHDatabaseSelector _multiDatabaseSelector = new();
        internal readonly BHLogger _logger;

        internal BHDatabaseBuilder()
        {
            _logger = BHDataProviderSelector._logger;
        }

        internal bool DropDatabase(string databaseName)
        {
            string databaseLocation = _multiDatabaseSelector.GetDatabasePath(databaseName);

            try
            {
                if (File.Exists(databaseLocation))
                {
                    SqliteConnection.ClearPool(new SqliteConnection(databaseName.BuildConnectionString()));
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(databaseLocation);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database Name: {databaseName} \n\n Error: {ex.ToString()}", $"Database_Drop");
                return false;
            }
        }

        internal bool CreateDatabase(string databaseName)
        {
            string databaseLocation = _multiDatabaseSelector.GetDatabasePath(databaseName);

            try
            {
                if (!File.Exists(databaseLocation))
                {
                    var stream = File.Create(databaseLocation);
                    stream.Dispose();
                    DatabaseStatics.InitializeData = true;
                }
                else
                {
                    DatabaseStatics.InitializeData = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database Name: {databaseName} \n\n Error: {ex.ToString()}", $"Database_Create");
                return false;
            }
        }

        internal bool DoesDbExists(string databaseName)
        {
            string databaseLocation = _multiDatabaseSelector.GetDatabasePath(databaseName);

            try
            {
                if (!File.Exists(databaseLocation))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Database Name: {databaseName} \n\n Error: {ex.ToString()}", $"Database_Chek");
                return false;
            }
        }

        internal bool IsCreatedFirstTime()
        {
            return DatabaseStatics.InitializeData;
        }
    }
}
