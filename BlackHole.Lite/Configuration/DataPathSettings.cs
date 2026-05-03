namespace BlackHole.Lite.Configuration
{
    /// <summary>
    /// Configures the file system folder where SQLite database files are stored.
    /// This class is returned by <see cref="BlackHoleLiteSettings.AddDatabase(string)"/> and related methods,
    /// enabling fluent configuration chaining.
    /// </summary>
    public class DataPathSettings
    {
        internal string DataPath { get; set; } = string.Empty;

        /// <summary>
        /// Sets the folder path where all SQLite database files for this configuration will be created and stored.
        /// If not called, the default location is <c>&lt;ApplicationBaseDirectory&gt;/BlackHoleData</c>.
        /// </summary>
        /// <param name="dataPath">An absolute or relative folder path. The folder will be created if it does not exist.
        /// Example: <c>@"C:\MyApp\Databases"</c> or <c>"./data"</c> or <c>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp")</c></param>
        /// <remarks>
        /// This setting is applied globally to all databases registered in the same SuperNova configuration.
        /// The method returns void to allow chaining with other fluent APIs if needed (though typically this is the final step
        /// in the configuration chain).
        /// </remarks>
        /// <example>
        /// <code>
        /// BlackHoleConfiguration.SuperNova(settings =>
        /// {
        ///     settings.AddDatabase("MyDb").SetDataPath(@"C:\ProgramData\MyApp\Databases");
        /// });
        /// </code>
        /// </example>
        public void SetDataPath(string dataPath)
        {
            DataPath = dataPath;
        }
    }
}
