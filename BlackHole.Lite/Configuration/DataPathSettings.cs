namespace BlackHole.Lite.Configuration
{
    public class DataPathSettings
    {
        internal string DataPath { get; set; } = string.Empty;

        public void SetDataPath(string dataPath)
        {
            DataPath = dataPath;
        }
    }
}
