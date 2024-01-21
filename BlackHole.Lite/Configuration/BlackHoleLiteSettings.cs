
namespace BlackHole.Lite.Configuration
{
    public class BlackHoleLiteSettings
    {

        internal DataPathSettings DataPath { get; set; } = new();
        internal List<DatabaseSettings> DbSettings { get; set; } = new();


        public DataPathSettings AddDatabase(string databaseName)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, string.Empty));
            return DataPath;
        }

        public DataPathSettings AddDatabase(string databaseName, string useNamespace)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, useNamespace));
            return DataPath;
        }

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
