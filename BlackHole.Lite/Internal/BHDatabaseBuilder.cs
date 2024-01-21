using BlackHole.CoreSupport;
using BlackHole.Logger;
using BlackHole.Statics;
using Microsoft.Data.Sqlite;

namespace BlackHole.Internal
{
    internal class BHDatabaseBuilder
    {
        private readonly BHDatabaseSelector _multiDatabaseSelector = new();

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
                Task.Factory.StartNew(() => databaseLocation.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString(), $"Drop {databaseName}"));
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
                Task.Factory.StartNew(() => databaseLocation.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString(), $"Create {databaseName}"));
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
                Task.Factory.StartNew(() => databaseLocation.CreateErrorLogs("DatabaseBuilder", ex.Message, ex.ToString(), $"Check {databaseName}"));
                return false;
            }
        }

        internal bool IsCreatedFirstTime()
        {
            return DatabaseStatics.InitializeData;
        }
    }
}
