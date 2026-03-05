namespace BlackHole.Lite.Logger
{
    internal class BHLogger
    {
        private void Log(string level, string? message, string? category)
        {
            DateTime utc = DateTime.UtcNow;

            string title = category == null ? string.Empty : $" title={category}";
            string log = $"[{utc.ToString("o")}] lvl={level}{title} \n msg='{message}' \n";
            Console.WriteLine(log);
        }

        internal void LogError(string? error, string? category = null)
        {
            Log("error", error, category);
        }

        internal void LogInfo(string? info, string? category = null)
        {
            Log("info", info, category);
        }

        internal void LogWarning(string? warning, string? category = null)
        {
            Log("warn", warning, category);
        }
    }
}
