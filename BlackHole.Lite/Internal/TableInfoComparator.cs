using BlackHole.DataProviders;
using BlackHole.Entities;

namespace BlackHole.Internal
{
    internal class TableInfoComparator
    {
        Type TableType { get; set; }

        internal readonly List<ColumnInfo> Columns = new();

        internal readonly List<FKInfo> ForeignKeys = new();

        internal readonly List<UniqueInfo> UniqueIndices = new();

        internal readonly List<IndexInfo> Indices = new();

        internal TableInfoComparator(Type tableType)
        {
            TableType = tableType;
        }

        public TableCompleteInfo GatherExistingInfo(SqliteExecutionProvider connection)
        {
            List<SQLiteIndexInfo> IndicesInfo = connection.Query<SQLiteIndexInfo>($"PRAGMA index_list({TableType.Name});", null);
            List<SQLiteTableInfo> TableInfo = connection.Query<SQLiteTableInfo>($"PRAGMA table_info({TableType.Name}); ", null);
            List<SQLiteForeignKeySchema> SchemaInfo = connection.Query<SQLiteForeignKeySchema>($"PRAGMA foreign_key_list({TableType.Name});", null);

            foreach (var column in TableInfo)
            {
                bool primaryKey = column.pk == 1;

                (Type dataType, Type baseType) = ParseColumnType(column.type, !column.notnull, primaryKey);

                Columns.Add(new ColumnInfo
                {
                    PropertyName = column.name,
                    IsNullable = primaryKey ? false : !column.notnull,
                    IsPrimaryKey = primaryKey,
                    IsCompositeKey = column.pk == 2,
                    PropertyType = dataType,
                    PropertyBaseType = baseType          
                });
            }

            foreach(var fkInfo in SchemaInfo)
            {
                ForeignKeys.Add(new FKInfo
                {
                    PropertyName = fkInfo.from,
                    ReferencedColumn = "Id",
                    ReferencedTable = fkInfo.to,
                    IsNullable = !TableInfo.First(c => c.name == fkInfo.from).notnull,
                    OnDelete = ParseBehaviour(fkInfo.on_delete)
                });
            }

            int groupIndex = 256;
            int defIndex = 0;

            foreach(var index in IndicesInfo)
            {
                var columns = connection.Query<SQLiteIndexColumnInfo>($"PRAGMA index_info({index.name});", null);

                string[] indexParts = index.name.Split('_');

                if (index.unique)
                {
                    int groupId = TryGetGroupId(indexParts);

                    if (groupId < 0)
                    {
                        groupId = defIndex;
                        defIndex++;
                    }

                    foreach (var column in columns)
                    {
                        UniqueIndices.Add(new UniqueInfo()
                        {
                            PropertyName = column.name,
                            GroupId = groupId,
                        });
                    }
                }
                else
                {
                    int groupId = TryGetGroupId(indexParts);

                    if (groupId < 0)
                    {
                        groupId = groupIndex;
                        groupIndex++;
                    }

                    Indices.Add(new IndexInfo(columns.Select(x => x.name).ToArray(), groupId));
                }
            }

            return new TableCompleteInfo(TableType)
            {
                Columns = Columns,
                ForeignKeys = ForeignKeys,
                Indices = Indices,
                UniqueIndices = UniqueIndices
            };
        }

        public int TryGetGroupId(string[] parts)
        {
            if (parts.Length > 1)
            {
                if (int.TryParse(parts[1], out var groupId))
                {
                    return groupId;
                }
            }

            return -1;
        }

        OnDeleteBehavior ParseBehaviour(string value)
        {
            switch (value.Replace(" ", "").ToUpper())
            {
                case "CASCADE":
                    return OnDeleteBehavior.Cascade;
                case "SETNULL":
                    return OnDeleteBehavior.SetNull;
                default:
                    return OnDeleteBehavior.Restrict;
            }
        }

        (Type, Type) ParseColumnType(string dataType, bool nullable, bool pk)
        {
            if(pk) return(typeof(int), typeof(int));

            int length = GetSqLiteLength(dataType);
            string type = dataType.ToLower().Split('(')[0];
            return type switch
            {
                "bigint" => (nullable ? typeof(long?) : typeof(long), typeof(long)),
                "bit" => (nullable ? typeof(bool?) : typeof(bool), typeof(bool)),
                "boolean" => (nullable ? typeof(bool?) : typeof(bool), typeof(bool)),
                "char" => (nullable ? typeof(char?) : typeof(char), typeof(char)),
                "date" => (nullable ? typeof(DateTime?) : typeof(DateTime), typeof(DateTime)),
                "datetime" => (nullable ? typeof(DateTime?) : typeof(DateTime), typeof(DateTime)),
                "decimal" => (nullable ? typeof(decimal?) : typeof(decimal), typeof(decimal)),
                "float" => (nullable ? typeof(float?) : typeof(float), typeof(float)),
                "real" => (nullable ? typeof(double?) : typeof(double), typeof(double)),
                "numeric" => (nullable ? typeof(double?) : typeof(double), typeof(double)),
                "int" => (nullable ? typeof(int?) : typeof(int), typeof(int)),
                "integer" => (nullable ? typeof(int?) : typeof(int), typeof(int)),
                "int2" => (nullable ? typeof(short?) : typeof(short), typeof(short)),
                "mediumint" => (nullable ? typeof(int?) : typeof(int), typeof(int)),
                "smallint" => (nullable ? typeof(short?) : typeof(short), typeof(short)),
                "tinyint" => (nullable ? typeof(byte?) : typeof(byte), typeof(byte)),
                "time" => (nullable ? typeof(DateTime?) : typeof(DateTime), typeof(DateTime)),
                "timestamp" => (nullable ? typeof(DateTime?) : typeof(DateTime), typeof(DateTime)),
                "blob" => (typeof(byte[]), typeof(byte[])),
                "varbinary" => (typeof(byte[]), typeof(byte[])),
                "longtext" => (typeof(string), typeof(string)),
                "mediumtext" => (typeof(string), typeof(string)),
                "tinytext" => (typeof(string), typeof(string)),
                "text" => length == 25 ? (nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset), typeof(DateTimeOffset))
                              : length == 36 ? (nullable ? typeof(Guid?) : typeof(Guid), typeof(Guid))
                              : (typeof(string), typeof(string)),
                "varchar" => length == 36 ? (nullable ? typeof(Guid?) : typeof(Guid), typeof(Guid))
                              : length == 25 ? (nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset), typeof(DateTimeOffset))
                              : (typeof(string), typeof(string)),
                _ => throw new Exception("Unrecognized Data Type on the Database")
            };
        }

        int GetSqLiteLength(string dataType)
        {
            int start = dataType.IndexOf('(');
            int end = dataType.IndexOf(')');

            if (start >= 0 && end > start)
            {
                if (int.TryParse(dataType.Substring(start + 1, end - start - 1), out int length))
                    return length;
            }

            return -1;
        }
    }
}
