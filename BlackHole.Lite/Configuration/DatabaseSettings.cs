

namespace BlackHole.Lite.Configuration
{
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

        public void AddDatabase(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public void AddDatabase(string databaseName, string selectedNamespace)
        {
            DatabaseName = databaseName;
            UsingNamespace = selectedNamespace;
        }
    }
}
