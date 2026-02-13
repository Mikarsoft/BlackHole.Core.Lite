

namespace BlackHole.Lite.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseSettings
    {
        internal string DatabaseName { get; set; }
        internal string UsingNamespace {  get; set; }
        
        internal string ConnectionString { get; set; } = string.Empty;

        internal DatabaseSettings()
        {
            DatabaseName = string.Empty;
            UsingNamespace = string.Empty;
        }

        internal DatabaseSettings(string databaseName, string selectedNamespace)
        {
            DatabaseName = databaseName;
            UsingNamespace = selectedNamespace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public void AddDatabase(string databaseName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="selectedNamespace"></param>
        public void AddDatabase(string databaseName, string selectedNamespace)
        {
            DatabaseName = databaseName;
            UsingNamespace = selectedNamespace;
        }
    }
}
