﻿using BlackHole.CoreSupport;
using BlackHole.Logger;
using Microsoft.Data.Sqlite;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class SqliteExecutionProvider
    {
        #region Constructor
        private string _connectionString;

        internal SqliteExecutionProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        public void SwitchConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region ExecutionMethods
        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                G? Id = default;
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (G?)Convert.ChangeType(Result, typeof(G));
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("Scalar", ex.Message, ex.ToString(), _connectionString));
                return default;
            }
        }

        public G? ExecuteScalar<G>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection connection = bhTransaction.connection as SqliteConnection;
                SqliteTransaction transaction = bhTransaction._transaction as SqliteTransaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (G?)Convert.ChangeType(Result, typeof(G));
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("Scalar", ex.Message, ex.ToString(), bhTransaction.connection.ConnectionString));
            }
            return default;
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQuery", ex.Message, ex.ToString(), _connectionString));
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection connection = bhTransaction.connection;
                SqliteTransaction transaction = bhTransaction._transaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs("NonQuery", ex.Message, ex.ToString(), bhTransaction.connection.ConnectionString));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                T? result = default;
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = Command.ExecuteReader())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result = line;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", ex.Message, ex.ToString(), _connectionString));
                return default;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                T? result = default;
                SqliteConnection connection = bHTransaction.connection;
                SqliteTransaction transaction = bHTransaction._transaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = Command.ExecuteReader())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result = line;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bHTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"QueryFirst_{typeof(T).Name}", ex.Message, ex.ToString(), _connectionString));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters)
        {
            try
            {
                List<T> result = new();
                using (SqliteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = Command.ExecuteReader())
                    {
                        while (DataReader.Read())
                        {
                            T? line = MapObject<T>(DataReader);

                            if (line != null)
                            {
                                result.Add(line);
                            }
                        }
                    }
                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString(), _connectionString));
                return new List<T>();
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction)
        {
            try
            {
                List<T> result = new();
                SqliteConnection connection = bHTransaction.connection;
                SqliteTransaction transaction = bHTransaction._transaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ArrayToParameters(parameters, Command.Parameters);

                using (SqliteDataReader DataReader = Command.ExecuteReader())
                {
                    while (DataReader.Read())
                    {
                        T? line = MapObject<T>(DataReader);

                        if (line != null)
                        {
                            result.Add(line);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bHTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Query_{typeof(T).Name}", ex.Message, ex.ToString(), bHTransaction.connection.ConnectionString));
                return new List<T>();
            }
        }
        #endregion

        #region ObjectMapping

        private T? MapObject<T>(SqliteDataReader reader)
        {
            try
            {
                Type type = typeof(T);

                if (type == typeof(string))
                {
                    if (reader.FieldCount > 0) return (T?)reader.GetValue(0);
                    else return default;
                }

                PropertyInfo[] properties = type.GetProperties();

                if (properties.Length == 0 && reader.FieldCount > 0)
                {
                    if (typeof(T) == typeof(Guid))
                    {
                        object GValue = reader.GetGuid(0);
                        return (T?)GValue;
                    }

                    object value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T?)Convert.ChangeType(value, typeof(T));
                    }

                    return default;
                }

                object? obj = Activator.CreateInstance(type);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {

                        PropertyInfo? property = properties.FirstOrDefault(x => string.Equals(x.Name, reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {
                            Type compairType = property.PropertyType;

                            if (property.Name.Contains("Nullable"))
                            {
                                if (property.PropertyType.GenericTypeArguments != null && property.PropertyType.GenericTypeArguments.Length > 0)
                                {
                                    compairType = property.PropertyType.GenericTypeArguments[0];
                                }
                            }

                            if (compairType == typeof(Guid))
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, reader.GetGuid(i));
                            }
                            else
                            {
                                type.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(reader.GetValue(i), property.PropertyType));
                            }
                        }
                    }
                }
                return (T?)obj;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => ex.Message.CreateErrorLogs($"Object_Mapping_{typeof(T).Name}", "MapperError", ex.ToString(), "Mapper"));
                return default;
            }
        }

        private void ArrayToParameters(List<BlackHoleParameter>? bhParameters, SqliteParameterCollection parameters)
        {
            if (bhParameters != null)
            {
                foreach (BlackHoleParameter param in bhParameters)
                {
                    object? value = param.Value;

                    if (value != null)
                    {
                        if (value.GetType() == typeof(Guid))
                        {
                            parameters.Add(new SqliteParameter(@param.Name, value.ToString()));
                        }
                        else
                        {
                            parameters.Add(new SqliteParameter(@param.Name, value));
                        }
                    }
                    else
                    {
                        parameters.Add(new SqliteParameter(@param.Name, DBNull.Value));
                    }
                }
            }
        }
        #endregion
    }
}
