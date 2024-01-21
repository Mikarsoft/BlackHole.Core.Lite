using BlackHole.DataProviders;

namespace BlackHole.Internal
{
    internal class BHDatabaseInfoReader
    {
        private SqliteExecutionProvider connection { get; set; }

        internal BHDatabaseInfoReader(SqliteExecutionProvider _connection)
        {
            connection = _connection;
        }

        internal void SwitchConnection(string connectionString)
        {
            connection.SwitchConnectionString(connectionString);
        }

        internal List<TableParsingInfo> GetDatabaseParsingInfo()
        {
            List<TableParsingInfo> parsingLiteData = new List<TableParsingInfo>();
            List<string> tableNames = connection.Query<string>("SELECT name FROM SQLite_master  where type = 'table' and name != 'SQLite_sequence';", null);
            foreach (string tableName in tableNames)
            {
                List<SQLiteTableInfo> tableInfo = connection.Query<SQLiteTableInfo>($"PRAGMA table_info({tableName});", null);
                List<SQLiteForeignKeySchema> foreignKeys = connection.Query<SQLiteForeignKeySchema>($"PRAGMA foreign_key_list({tableName});", null);
                LiteAutoIncrementInfo? isAutoIncrement = connection.QueryFirst<LiteAutoIncrementInfo>($"SELECT * FROM SQLite_sequence WHERE name='{tableName}'", null);
                foreach (SQLiteTableInfo info in tableInfo)
                {
                    TableParsingInfo parsingLine = new TableParsingInfo
                    {
                        TableName = tableName,
                        ColumnName = info.name,
                        DataType = info.type,
                        Nullable = !info.notnull,
                        PrimaryKey = info.pk > 0,
                        DefaultValue = info.dflt_value
                    };
                    if (info.pk == 1 && isAutoIncrement != null && (info.type.ToLower().Contains("integer") || info.type.ToLower().Contains("bigint")))
                    {
                        parsingLine.Extra = "auto_increment";
                    }

                    if (info.type.ToLower().Contains("bigint") && info.dflt_value.ToLower().Contains("last_insert_rowid()"))
                    {
                        parsingLine.Extra = "auto_increment";
                        parsingLine.PrimaryKey = true;
                    }

                    SQLiteForeignKeySchema? foreignKey = foreignKeys.Where(x => x.from == parsingLine.ColumnName).FirstOrDefault();
                    if (foreignKey != null)
                    {
                        parsingLine.DeleteRule = foreignKey.on_delete;
                        parsingLine.ReferencedTable = foreignKey.table;
                        parsingLine.ReferencedColumn = foreignKey.to;
                    }
                    parsingLiteData.Add(parsingLine);
                }
            }
            return parsingLiteData;
        }
    }
}
