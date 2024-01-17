using BlackHole.CoreSupport;
using BlackHole.DataProviders;
using System.Reflection;

namespace BlackHole.Core
{
    /// <summary>
    /// An Interface that gives all
    /// the required methods to perform custom sql commands
    /// <para>It's already registered in the ServiceCollection and it can be used to 
    /// your services with Dependency Injection</para>
    /// <para>The connection is automatically generated and disposed after 
    /// each execution</para>
    /// </summary>
    public class BHConnection
    {
        private readonly SqliteExecutionProvider _executionProvider;

        /// <summary>
        /// An Interface that gives all
        /// the required methods to perform custom sql commands
        /// <para>It's already registered in the ServiceCollection and it can be used to 
        /// your services with Dependency Injection</para>
        /// <para>The connection is automatically generated and disposed after 
        /// each execution</para>
        /// </summary>
        internal BHConnection()
        {
            _executionProvider = BHDataProviderSelector.GetExecutionProvider();
        }

        /// <summary>
        /// <para>Classic Execute Scalar that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null);
        }

        /// <summary>
        /// <para> Classic Execute Scalar with BHParameters that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHParameters parameters)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// <para> Classic Execute Scalar with Object as Parameters that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, object parametersObject)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <para><b>Transaction.</b> Classic Execute Scalar with BHParameters that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <para><b>Transaction.</b> Classic Execute Scalar that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <para><b>Transaction.</b> Classic Execute Scalar with Object as Parameters that returns the first value of the result</para>
        /// </summary>
        /// <typeparam name="G">Output Value Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Value of the Result</returns>
        public G? ExecuteScalar<G>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.ExecuteScalar<G>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// Classic Execute with BHParameters.
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>Success</returns>
        public bool JustExecute(string commandText, BHParameters parameters)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters);
        }

        /// <summary>
        /// Classic Execute with Object as Parameters
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>Success</returns>
        public bool JustExecute(string commandText, object parametersObject)
        {
            return _executionProvider.JustExecute(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <b>Transaction.</b> Classic Execute without output.
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        public bool JustExecute(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> Classic Execute with BHParameters.
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        public bool JustExecute(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> Classic Execute with Object as Parameters.
        /// </summary>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>Success</returns>
        public bool JustExecute(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.JustExecute(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// A Query that returns returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText)
        {
            return _executionProvider.Query<T>(commandText, null);
        }

        /// <summary>
        /// A Query that takes BHParameters and returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, object parametersObject)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that takes BHParameters and returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that takes an Object as parameters and returns all Lines of the Result as List.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public List<T> Query<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.Query<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        /// <summary>
        /// A Query that returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText)
        {
            return _executionProvider.QueryFirst<T>(commandText, null);
        }

        /// <summary>
        /// A Query that takes BHParameters and returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHParameters parameters)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters);
        }

        /// <summary>
        /// A Query that takes an Object as parameters and returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, object parametersObject)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject));
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, null, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that takes BHParameters and returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parameters">BHParameters Class, populated with black hole parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, BHParameters parameters, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, parameters.Parameters, bHTransaction.transaction);
        }

        /// <summary>
        /// <b>Transaction.</b> A Query that takes an Object as parameters and returns only the first Line of the result.
        /// </summary>
        /// <typeparam name="T">Output Type</typeparam>
        /// <param name="commandText">Command Text</param>
        /// <param name="parametersObject">Class with properties as Parameters</param>
        /// <param name="bHTransaction">BHTransaction Class, contains connection and transaction</param>
        /// <returns>The First Line of the Result</returns>
        public T? QueryFirst<T>(string commandText, object parametersObject, BHTransaction bHTransaction)
        {
            return _executionProvider.QueryFirst<T>(commandText, MapObjectToParameters(parametersObject), bHTransaction.transaction);
        }

        private static List<BlackHoleParameter> MapObjectToParameters(object parametersObject)
        {
            PropertyInfo[] propertyInfos = parametersObject.GetType().GetProperties();
            BHParameters parameters = new();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(parametersObject);
                parameters.Add(property.Name, value);
            }
            return parameters.Parameters;
        }
    }
}
