using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Logger;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;

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
            where T : BHEntity
        {
            List<int> Ids = new();
            string commandText = $"{commandStart}) {commandEnd}) {insertedOutput};";

            foreach (T entry in entries)
            {
                entry.Id = ExecuteEntryScalar(commandText, entry, bhTransaction);
                Ids.Add(entry.Id);
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

        public List<TRoot> QueryWithIncludes<TRoot>(string commandText, List<BlackHoleParameter>? parameters,
     string connectionString, List<IncludePart> includes) where TRoot : BHEntity
        {
            var rootList = new List<TRoot>();
            var identityMap = new Dictionary<(Type, object), object>();
            var wiredRelationships = new HashSet<(object parent, int includeIndex, object child)>();

            var rootPropMap = typeof(TRoot).GetProperties()
                .Where(p => p.PropertyType.IsAllowedType())
                .ToDictionary(p => $"r_{p.Name}", p => p, StringComparer.OrdinalIgnoreCase);

            var includePropMaps = includes.Select((inc, i) =>
                inc.TableType.GetProperties()
                    .Where(p => p.PropertyType.IsAllowedType())
                    .ToDictionary(p => $"c{i}_{p.Name}", p => p, StringComparer.OrdinalIgnoreCase)
            ).ToArray();

            using (SqliteConnection connection = new(connectionString))
            {
                connection.Open();
                SqliteCommand command = new(commandText, connection);
                ArrayToParameters(parameters, command.Parameters);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var resolved = new object?[includes.Count + 1];

                        // --- Root ---
                        var rootIdVal = reader["r_Id"];
                        if (rootIdVal == null || rootIdVal == DBNull.Value) continue;

                        if (!identityMap.TryGetValue((typeof(TRoot), rootIdVal), out var rootObj))
                        {
                            rootObj = Activator.CreateInstance(typeof(TRoot))!;
                            MapProperties(reader, rootObj, rootPropMap);
                            identityMap[(typeof(TRoot), rootIdVal)] = rootObj;
                            rootList.Add((TRoot)rootObj);
                        }
                        resolved[0] = rootObj;

                        // --- Includes ---
                        for (int i = 0; i < includes.Count; i++)
                        {
                            var inc = includes[i];

                            object? parentObj = inc.ParentIndex == -1 ? resolved[0] : resolved[inc.ParentIndex + 1];

                            if (parentObj == null)
                            {
                                resolved[i + 1] = null;
                                continue;
                            }

                            var childIdVal = reader[$"c{i}_Id"];
                            if (childIdVal == null || childIdVal == DBNull.Value)
                            {
                                resolved[i + 1] = null;
                                continue;
                            }

                            if (!identityMap.TryGetValue((inc.TableType, childIdVal), out var childObj))
                            {
                                childObj = Activator.CreateInstance(inc.TableType)!;
                                MapProperties(reader, childObj, includePropMaps[i]);
                                identityMap[(inc.TableType, childIdVal)] = childObj;
                            }

                            resolved[i + 1] = childObj;

                            var relationKey = (parentObj, i, childObj);
                            if (!wiredRelationships.Contains(relationKey))
                            {
                                wiredRelationships.Add(relationKey);
                                WireRelationship(parentObj, inc, childObj);
                            }
                        }
                    }
                }
            }

            return rootList;
        }

        public List<TRoot> QueryWithIncludes<TRoot>(string commandText, List<BlackHoleParameter>? parameters, BlackHoleTransaction bHTransaction, List<IncludePart> includes)
        {
            var rootList = new List<TRoot>();
            var identityMap = new Dictionary<(Type, object), object>();
            var wiredRelationships = new HashSet<(object parent, int includeIndex, object child)>();

            var rootPropMap = typeof(TRoot).GetProperties()
                .Where(p => p.PropertyType.IsAllowedType())
                .ToDictionary(p => $"r_{p.Name}", p => p, StringComparer.OrdinalIgnoreCase);

            var includePropMaps = includes.Select((inc, i) =>
                inc.TableType.GetProperties()
                    .Where(p => p.PropertyType.IsAllowedType())
                    .ToDictionary(p => $"c{i}_{p.Name}", p => p, StringComparer.OrdinalIgnoreCase)
            ).ToArray();

            SqliteConnection connection = bHTransaction.connection;
            SqliteTransaction transaction = bHTransaction._transaction;
            SqliteCommand Command = new(commandText, connection, transaction);
            ArrayToParameters(parameters, Command.Parameters); ;

            using (var reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var resolved = new object?[includes.Count + 1];

                    // --- Root ---
                    var rootIdVal = reader["r_Id"];
                    if (rootIdVal == null || rootIdVal == DBNull.Value) continue;

                    if (!identityMap.TryGetValue((typeof(TRoot), rootIdVal), out var rootObj))
                    {
                        rootObj = Activator.CreateInstance(typeof(TRoot))!;
                        MapProperties(reader, rootObj, rootPropMap);
                        identityMap[(typeof(TRoot), rootIdVal)] = rootObj;
                        rootList.Add((TRoot)rootObj);
                    }
                    resolved[0] = rootObj;

                    // --- Includes ---
                    for (int i = 0; i < includes.Count; i++)
                    {
                        var inc = includes[i];

                        object? parentObj = inc.ParentIndex == -1 ? resolved[0] : resolved[inc.ParentIndex + 1];

                        if (parentObj == null)
                        {
                            resolved[i + 1] = null;
                            continue;
                        }

                        var childIdVal = reader[$"c{i}_Id"];
                        if (childIdVal == null || childIdVal == DBNull.Value)
                        {
                            resolved[i + 1] = null;
                            continue;
                        }

                        if (!identityMap.TryGetValue((inc.TableType, childIdVal), out var childObj))
                        {
                            childObj = Activator.CreateInstance(inc.TableType)!;
                            MapProperties(reader, childObj, includePropMaps[i]);
                            identityMap[(inc.TableType, childIdVal)] = childObj;
                        }

                        resolved[i + 1] = childObj;

                        var relationKey = (parentObj, i, childObj);
                        if (!wiredRelationships.Contains(relationKey))
                        {
                            wiredRelationships.Add(relationKey);
                            WireRelationship(parentObj, inc, childObj);
                        }
                    }
                }
            }

            return rootList;
        }
        #endregion

        #region Object Mapping

        private void MapProperties(SqliteDataReader reader, object obj, Dictionary<string, PropertyInfo> propMap)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i)) continue;

                string colName = reader.GetName(i);
                if (!propMap.TryGetValue(colName, out var property)) continue;

                Type compairType = property.PropertyType;

                if (property.PropertyType.Name.Contains("Nullable"))
                {
                    if (property.PropertyType.GenericTypeArguments?.Length > 0)
                    {
                        compairType = property.PropertyType.GenericTypeArguments[0];
                    }
                }

                switch (compairType)
                {
                    case Type t when t == typeof(Guid):

                        if (reader.GetGuid(i) is Guid guidValue)
                        {
                            property.SetValue(obj, guidValue);
                        }

                        break;

                    case Type t when t == typeof(DateTimeOffset):

                        if (reader.GetString(i) is string dtovalue)
                        {
                            if (DateTimeOffset.TryParseExact(dtovalue, "yyyy-MM-ddTHH:mm:sszzz",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                            {
                                property.SetValue(obj, result);
                            }
                        }

                        break;

                    case Type t when t == typeof(DateTime):

                        if (reader.GetDateTime(i) is DateTime dtValue)
                        {
                            property.SetValue(obj, dtValue);
                        }

                        break;

                    default:

                        if (reader.GetValue(i) is object value)
                        {
                            property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                        }

                        break;
                }
            }
        }

        private void WireRelationship(object parent, IncludePart inc, object child)
        {
            var navProp = parent.GetType().GetProperty(inc.NavigationPropertyName);
            if (navProp == null) return;

            if (inc.IsList)
            {
                var wrapper = navProp.GetValue(parent);
                if (wrapper == null)
                {
                    wrapper = Activator.CreateInstance(navProp.PropertyType)!;
                    navProp.SetValue(parent, wrapper);
                }
                navProp.PropertyType.GetMethod("Add")!.Invoke(wrapper, new[] { child });
            }
            else
            {
                var wrapper = navProp.GetValue(parent);
                if (wrapper == null)
                {
                    wrapper = Activator.CreateInstance(navProp.PropertyType)!;
                    navProp.SetValue(parent, wrapper);
                }
                navProp.PropertyType.GetProperty("Value")!.SetValue(wrapper, child);
            }
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

                            switch (compairType)
                            {
                                case Type t when t == typeof(Guid):

                                    if (reader.GetGuid(i) is Guid guidValue)
                                    {
                                        type.GetProperty(property.Name)?.SetValue(obj, guidValue);
                                    }

                                    break;

                                case Type t when t == typeof(DateTimeOffset):

                                    if (reader.GetString(i) is string dtovalue)
                                    {
                                        if (DateTimeOffset.TryParseExact(dtovalue, "yyyy-MM-ddTHH:mm:sszzz",
                                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                                        {
                                            type.GetProperty(property.Name)?.SetValue(obj, result);
                                        }
                                    }

                                    break;

                                case Type t when t == typeof(DateTime):

                                    if (reader.GetDateTime(i) is DateTime dtValue)
                                    {
                                        type.GetProperty(property.Name)?.SetValue(obj, dtValue);
                                    }

                                    break;   
                                    
                                default:

                                    if (reader.GetValue(i) is object value)
                                    {
                                        type.GetProperty(property.Name)?.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                                    }

                                    break;
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
                        var valueType = value?.GetType();

                        switch (valueType)
                        {
                            case Type t when t == typeof(Guid):
                                parameters.Add(new SqliteParameter(@param.Name, value?.ToString()));
                                break;
                            case Type t when t == typeof(DateTimeOffset):
                                if (value is DateTimeOffset dt)
                                {
                                    parameters.Add(new SqliteParameter(@param.Name, dt.ToString("yyyy-MM-ddTHH:mm:sszzz")));
                                }
                                else
                                {
                                    parameters.Add(new SqliteParameter(@param.Name, value));
                                }
                                break;
                            default:
                                parameters.Add(new SqliteParameter(@param.Name, value));
                                break;
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