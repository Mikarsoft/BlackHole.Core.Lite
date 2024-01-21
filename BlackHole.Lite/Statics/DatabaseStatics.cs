
namespace BlackHole.Statics
{
    internal static class DatabaseStatics
    {
        internal static string DefaultConnectionString { get; set; } = string.Empty;
        internal static string DefaultDatabaseName { get; set; } = string.Empty;
        internal static string DataPath { get; set; } = string.Empty;
        internal static bool UseLogging { get; set; } = true;
        internal static bool AutoUpdate { get; set; } = true;
        internal static bool InitializeData { get; set; } = false;
        internal static string DbDateFormat { get; set; } = "yyyy-MM-dd";
        internal static bool IsLiteLibInitialized { get; set; }
    }
}
