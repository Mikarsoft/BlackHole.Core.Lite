
namespace BlackHole.CoreSupport
{
    internal interface IExecutionProvider
    {
        G ExecuteScalar<G>(string commandText, List<BlackHoleParameter> parameters);
        G ExecuteScalar<G>(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bHTransaction);

        bool JustExecute(string commandText, List<BlackHoleParameter> parameters);
        bool JustExecute(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bHTransaction);

        T QueryFirst<T>(string commandText, List<BlackHoleParameter> parameters);
        T QueryFirst<T>(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction bHTransaction);

        List<T> Query<T>(string commandText, List<BlackHoleParameter> parameters);
        List<T> Query<T>(string commandText, List<BlackHoleParameter> parameters, BlackHoleTransaction BlackHoleTransaction);
    }
}
