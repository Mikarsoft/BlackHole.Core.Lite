using BlackHole.DataProviders;
using BlackHole.Internal;
using BlackHole.Lite.Entities;

namespace BlackHole.Lite.Internal
{
    internal class TableInfoComparator
    {
        Type TableType { get; set; }

        internal readonly List<ColumnInfo> Columns = new();

        internal readonly List<FKInfo> ForeignKeys = new();

        internal readonly List<UniqueInfo> UniqueIndices = new();

        internal readonly List<NonUniqueIndex> Indices = new();

        internal TableInfoComparator(Type tableType)
        {
            TableType = tableType;
        }

        public void GatherExistingInfo(SqliteExecutionProvider connection)
        {
            List<SQLiteIndexInfo> IndicesInfo = connection.Query<SQLiteIndexInfo>($"PRAGMA index_list({TableType.Name});", null);
            List<SQLiteTableInfo> TableInfo = connection.Query<SQLiteTableInfo>($"PRAGMA table_info({TableType.Name}); ", null);
            List<SQLiteForeignKeySchema> SchemaInfo = connection.Query<SQLiteForeignKeySchema>($"PRAGMA foreign_key_list({TableType.Name});", null);

            foreach (var column in TableInfo)
            {
                Columns.Add(new ColumnInfo
                {
                    PropertyName = column.name,
                    IsNullable = !column.notnull,
                    IsPrimaryKey = column.pk == 1,
                    IsCompositeKey = column.pk == 2
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

                if (index.origin == "c")
                {
                    int groupId = TryGetGroupId(indexParts);

                    if (groupId < 0)
                    {
                        groupId = groupIndex;
                        groupIndex++;
                    }

                    Indices.Add(new NonUniqueIndex(columns.Select(x => x.name).ToArray(), groupIndex));
                }

                if (index.origin == "u")
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
            }
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
    }
}
