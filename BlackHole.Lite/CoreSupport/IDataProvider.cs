
namespace BlackHole.CoreSupport
{
    internal interface IDataProvider
    {
        int InsertScalar<T>(string commandStart, string commandEnd, T entry);
        int InsertScalar<T>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction);

        List<int> MultiInsertScalar<T>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction);

        bool ExecuteEntry<T>(string commandText, T entry);
        bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction);

        bool JustExecute(string commandText, List<BlackHoleParameter> parameters);
        bool JustExecute(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bhTransaction);

        T QueryFirst<T>(string commandText, List<BlackHoleParameter> parameters);
        T QueryFirst<T>(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bHTransaction);

        List<T> Query<T>(string commandText, List<BlackHoleParameter> parameters);
        List<T> Query<T>(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bHTransaction);
    }
}
