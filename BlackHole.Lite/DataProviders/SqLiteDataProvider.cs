using BlackHole.CoreSupport;
using BlackHole.Logger;
using Microsoft.Data.Sqlite;
using System.Reflection;

namespace BlackHole.DataProviders
{
    internal class SqliteDataProvider
    {
        #region Constructor
        internal readonly string insertedOutput = "returning Id";
        internal readonly bool skipQuotes = true;

        internal SqliteDataProvider()
        {
        }
        #endregion

        #region Internal Processes
        private int ExecuteEntryScalar<T>(string commandText, T entry, string connectionString)
        {
            try
            {
                int Id = default;
                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    object? Result = Command.ExecuteScalar();
                    connection.Close();

                    if (Result != null)
                    {
                        Id = (int)Convert.ChangeType(Result, typeof(int));
                    }
                }
                return Id;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString(), connectionString));
                return default;
            }
        }

        private int ExecuteEntryScalar<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection;
                SqliteTransaction? transaction = bhTransaction._transaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                object? Result = Command.ExecuteScalar();

                if (Result != null)
                {
                    return (int)Convert.ChangeType(Result, typeof(int));
                }
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString(), bhTransaction.connection.ConnectionString));
            }
            return default;
        }
        #endregion

        #region Execution Methods
        public int InsertScalar<T>(string commandStart, string commandEnd, T entry, string connectionString)
        {
            return ExecuteEntryScalar($"{commandStart}){commandEnd}) {insertedOutput};", entry, connectionString);
        }

        public int InsertScalar<T>(string commandStart, string commandEnd, T entry, BlackHoleTransaction bhTransaction)
        {
            return ExecuteEntryScalar($"{commandStart}) {commandEnd}) {insertedOutput};", entry, bhTransaction);
        }

        public List<int> MultiInsertScalar<T>(string commandStart, string commandEnd, List<T> entries, BlackHoleTransaction bhTransaction)
        {
            List<int> Ids = new();
            string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";

            foreach (T entry in entries)
            {
                Ids.Add(ExecuteEntryScalar(commandText, entry, bhTransaction));
            }

            return Ids;
        }

        public bool ExecuteEntry<T>(string commandText, T entry, string connectionString)
        {
            try
            {
                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ObjectToParameters(entry, Command.Parameters);
                    Command.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString(), connectionString));
                return false;
            }
        }

        public bool ExecuteEntry<T>(string commandText, T entry, BlackHoleTransaction bhTransaction)
        {
            try
            {
                SqliteConnection? connection = bhTransaction.connection;
                SqliteTransaction? transaction = bhTransaction._transaction;
                SqliteCommand Command = new(commandText, connection, transaction);
                ObjectToParameters(entry, Command.Parameters);
                Command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                bhTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Insert_{typeof(T).Name}", ex.Message, ex.ToString(), bhTransaction.connection.ConnectionString));
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Execute", ex.Message, ex.ToString(), bhTransaction.connection.ConnectionString));
                return false;
            }
        }

        public bool JustExecute(string commandText, List<BlackHoleParameter>? parameters, string connectionString)
        {
            try
            {
                using (SqliteConnection connection = new(connectionString))
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Execute", ex.Message, ex.ToString(), connectionString));
                return false;
            }
        }

        public T? QueryFirst<T>(string commandText, List<BlackHoleParameter>? parameters, string connectionString)
        {
            try
            {
                T? result = default;

                using (SqliteConnection connection = new(connectionString))
                {
                    connection.Open();
                    SqliteCommand Command = new(commandText, connection);
                    ArrayToParameters(parameters, Command.Parameters);

                    using (SqliteDataReader DataReader = Command.ExecuteReader())
                    {
                        while (DataReader.Read())
                        {
                            result = MapObject<T>(DataReader);
                            break;
                        }
                    }
                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString(), connectionString));
                return default;
            }
        }

        public List<T> Query<T>(string commandText, List<BlackHoleParameter>? parameters, string connectionString)
        {
            try
            {
                List<T> result = new();

                using (SqliteConnection connection = new(connectionString))
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString(), connectionString));
                return new List<T>();
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
                        result = MapObject<T>(DataReader);
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                bHTransaction.hasError = true;
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"SelectFirst_{typeof(T).Name}", ex.Message, ex.ToString(), bHTransaction.connection.ConnectionString));
                return default;
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
                Task.Factory.StartNew(() => commandText.CreateErrorLogs($"Select_{typeof(T).Name}", ex.Message, ex.ToString(), bHTransaction.connection.ConnectionString));
                return new List<T>();
            }
        }

        public List<TRoot> QueryWithIncludes<TRoot>(string commandText, List<BlackHoleParameter>? parameters, string connectionString, List<IncludeInfo> includes)
        {
            var rootList = new List<TRoot>();
            var identityMap = new Dictionary<(Type, object), object>();
            var wiredRelationships = new HashSet<(object parent, string nav, object child)>();

            using (SqliteConnection connection = new(connectionString))
            {
                connection.Open();
                SqliteCommand Command = new(commandText, connection);
                ArrayToParameters(parameters, Command.Parameters);

                using var reader = Command.ExecuteReader();

                var rootColumns = BuildColumnMap(reader, typeof(TRoot), prefix: null);
                var includeColumns = includes.Select(inc =>
                    BuildColumnMap(reader, inc.EntityType, inc.ColumnPrefix)).ToList();

                var parentColumnMaps = includes.Select(inc =>
                {
                    if (inc.ParentType == null) return null;
                    return BuildColumnMap(reader, inc.ParentType, inc.ParentPkPrefix);
                }).ToList();

                while (reader.Read())
                {
                    var rootPk = reader.GetValue(rootColumns.PkIndex);
                    var rootKey = (typeof(TRoot), rootPk);

                    if (!identityMap.TryGetValue(rootKey, out var rootObj))
                    {
                        rootObj = MapColumns(reader, typeof(TRoot), rootColumns);

                        if (rootObj != null)
                        {
                            identityMap[rootKey] = rootObj;
                            rootList.Add((TRoot)rootObj);
                        }
                    }

                    for (int i = 0; i < includes.Count; i++)
                    {
                        var inc = includes[i];
                        var cols = includeColumns[i];

                        if (reader.IsDBNull(cols.PkIndex)) continue;

                        var relatedPk = reader.GetValue(cols.PkIndex);
                        var relatedKey = (inc.EntityType, relatedPk);

                        if (!identityMap.TryGetValue(relatedKey, out var relatedObj))
                        {
                            relatedObj = MapColumns(reader, inc.EntityType, cols);

                            if (relatedObj != null)
                            {
                                identityMap[relatedKey] = relatedObj;
                            }
                        }

                        object? parentObj;

                        if (inc.ParentType == null)
                        {
                            parentObj = rootObj;
                        }
                        else
                        {
                            var parentMap = parentColumnMaps[i];

                            if (parentMap == null || reader.IsDBNull(parentMap.PkIndex))
                            {
                                continue;
                            }

                            var parentPk = reader.GetValue(parentMap.PkIndex);
                            identityMap.TryGetValue((inc.ParentType, parentPk), out parentObj);
                        }

                        if (parentObj != null && relatedObj != null)
                        {
                            if (inc.IsCollection)
                            {
                                var wireKey = (parentObj, inc.NavigationProperty!.Name, relatedObj);
                                if (wiredRelationships.Add(wireKey))
                                {
                                    inc.AddToCollection(parentObj, relatedObj);
                                }
                            }
                            else
                            {
                                inc.SetReference(parentObj, relatedObj);
                            }
                        }
                    }
                }
            }

            return rootList;
        }

        public List<TRoot> QueryWithIncludes<TRoot>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, List<IncludeInfo> includes)
        {
            var rootList = new List<TRoot>();
            var identityMap = new Dictionary<(Type, object), object>();
            var wiredRelationships = new HashSet<(object parent, string nav, object child)>();

            SqliteConnection connection = bHTransaction.connection;
            SqliteTransaction transaction = bHTransaction._transaction;
            SqliteCommand Command = new(commandText, connection, transaction);
            ArrayToParameters(parameters, Command.Parameters); ;

            using var reader = Command.ExecuteReader();

            var rootColumns = BuildColumnMap(reader, typeof(TRoot), prefix: null);
            var includeColumns = includes.Select(inc =>
                BuildColumnMap(reader, inc.EntityType, inc.ColumnPrefix)).ToList();

            var parentColumnMaps = includes.Select(inc =>
            {
                if (inc.ParentType == null) return null;
                return BuildColumnMap(reader, inc.ParentType, inc.ParentPkPrefix);
            }).ToList();

            while (reader.Read())
            {
                var rootPk = reader.GetValue(rootColumns.PkIndex);
                var rootKey = (typeof(TRoot), rootPk);

                if (!identityMap.TryGetValue(rootKey, out var rootObj))
                {
                    rootObj = MapColumns(reader, typeof(TRoot), rootColumns);

                    if (rootObj != null)
                    {
                        identityMap[rootKey] = rootObj;
                        rootList.Add((TRoot)rootObj);
                    }
                }

                for (int i = 0; i < includes.Count; i++)
                {
                    var inc = includes[i];
                    var cols = includeColumns[i];

                    if (reader.IsDBNull(cols.PkIndex)) continue;

                    var relatedPk = reader.GetValue(cols.PkIndex);
                    var relatedKey = (inc.EntityType, relatedPk);

                    if (!identityMap.TryGetValue(relatedKey, out var relatedObj))
                    {
                        relatedObj = MapColumns(reader, inc.EntityType, cols);

                        if (relatedObj != null)
                        {
                            identityMap[relatedKey] = relatedObj;
                        }
                    }

                    object? parentObj;

                    if (inc.ParentType == null)
                    {
                        parentObj = rootObj;
                    }
                    else
                    {
                        var parentMap = parentColumnMaps[i];

                        if (parentMap == null || reader.IsDBNull(parentMap.PkIndex))
                        {
                            continue;
                        }

                        var parentPk = reader.GetValue(parentMap.PkIndex);
                        identityMap.TryGetValue((inc.ParentType, parentPk), out parentObj);
                    }

                    if (parentObj != null && relatedObj != null)
                    {
                        if (inc.IsCollection)
                        {
                            var wireKey = (parentObj, inc.NavigationProperty!.Name, relatedObj);
                            if (wiredRelationships.Add(wireKey))
                            {
                                inc.AddToCollection(parentObj, relatedObj);
                            }
                        }
                        else
                        {
                            inc.SetReference(parentObj, relatedObj);
                        }
                    }
                }
            }

            return rootList;
        }
        #endregion

        #region Object Mapping

        private EntityColumnMap BuildColumnMap(SqliteDataReader reader, Type entityType, string? prefix)
        {
            var props = entityType.GetProperties();
            var columnIndexes = new Dictionary<PropertyInfo, int>();
            int pkIndex = -1;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string colName = reader.GetName(i);

                // If using prefixed aliases like "Customer_Id", strip the prefix
                if (prefix != null)
                {
                    if (!colName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
                    colName = colName.Substring(prefix.Length);
                }

                var prop = props.FirstOrDefault(p =>
                    string.Equals(p.Name, colName, StringComparison.OrdinalIgnoreCase));

                if (prop != null)
                {
                    columnIndexes[prop] = i;
                    if (prop.Name == "Id") pkIndex = i; // or use [Key] attribute detection
                }
            }

            return new EntityColumnMap { PkIndex = pkIndex, Columns = columnIndexes };
        }

        private object? MapColumns(SqliteDataReader reader, Type type, EntityColumnMap columnMap)
        {
            object? obj = Activator.CreateInstance(type);

            foreach (var (property, columnIndex) in columnMap.Columns)
            {
                if (reader.IsDBNull(columnIndex)) continue;

                Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                if (targetType == typeof(Guid))
                {
                    property.SetValue(obj, reader.GetGuid(columnIndex));
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(reader.GetValue(columnIndex), targetType));
                }
            }

            return obj;
        }

        private T? MapObject<T>(SqliteDataReader reader)
        {
            try
            {
                Type type = typeof(T);

                if (type == typeof(string))
                {
                    if (reader.FieldCount > 0) return (T)reader.GetValue(0);
                    else return default;
                }

                PropertyInfo[] properties = type.GetProperties();

                if (properties.Length == 0 && reader.FieldCount > 0)
                {
                    if (typeof(T) == typeof(Guid))
                    {
                        object GValue = reader.GetGuid(0);
                        return (T)GValue;
                    }

                    object value = reader.GetValue(0);

                    if (value != null)
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
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

                            if (property.PropertyType.Name.Contains("Nullable"))
                            {
                                if (property.PropertyType.GenericTypeArguments != null && property.PropertyType.GenericTypeArguments.Length > 0)
                                {
                                    compairType = property.PropertyType.GenericTypeArguments[0];
                                }
                            }

                            if (compairType == typeof(Guid))
                            {
                                if(reader.GetGuid(i) is Guid guidValue)
                                {
                                    type.GetProperty(property.Name)?.SetValue(obj, guidValue);
                                }
                            }
                            else if(compairType == typeof(DateTime))
                            {
                                if(reader.GetDateTime(i) is DateTime dtValue)
                                {
                                    type.GetProperty(property.Name)?.SetValue(obj, dtValue);
                                }
                            }
                            else
                            {
                                if(reader.GetValue(i) is object value)
                                {
                                    type.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                                }
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

        private void ObjectToParameters<T>(T item, SqliteParameterCollection parameters)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            foreach (PropertyInfo property in propertyInfos)
            {
                object? value = property.GetValue(item);

                if (value != null)
                {
                    var valueType = value?.GetType();

                    switch (valueType)
                    {
                        case Type t when t == typeof(Guid):
                            parameters.Add(new SqliteParameter(@property.Name, value?.ToString()));
                            break;
                        case Type t when t == typeof(DateTimeOffset):
                            if (value is DateTimeOffset dt)
                            {
                                parameters.Add(new SqliteParameter(@property.Name, dt.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                            }
                            else
                            {
                                parameters.Add(new SqliteParameter(@property.Name, value));
                            }
                            break;
                        default:
                            parameters.Add(new SqliteParameter(@property.Name, value));
                            break;
                    }
                }
                else
                {
                    parameters.Add(new SqliteParameter(@property.Name, DBNull.Value));
                }
            }
        }
        #endregion
    }
}