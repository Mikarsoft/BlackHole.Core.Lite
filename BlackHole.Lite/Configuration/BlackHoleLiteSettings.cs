
namespace BlackHole.Lite.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class BlackHoleLiteSettings
    {

        internal DataPathSettings DataPath { get; set; } = new();
        internal List<DatabaseSettings> DbSettings { get; set; } = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataPathSettings AddDatabase(string databaseName)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, string.Empty));
            return DataPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="useNamespace"></param>
        /// <returns></returns>
        public DataPathSettings AddDatabase(string databaseName, string useNamespace)
        {
            DbSettings.Add(new DatabaseSettings(databaseName, useNamespace));
            return DataPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multipleDatabase"></param>
        /// <returns></returns>
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
