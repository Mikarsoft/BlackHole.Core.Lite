using BlackHole.DataProviders;
using BlackHole.Statics;

namespace BlackHole.Internal
{
    internal class BHDatabaseSelector
    {
        internal SqliteExecutionProvider GetExecutionProvider(string connectionString)
        {
            return new SqliteExecutionProvider(connectionString);
        }

        internal string GetPrimaryKeyCommand()
        {
            return "Id INTEGER PRIMARY KEY AUTOINCREMENT ,";
        }

        internal string GetServerConnection(string databaseName)
        {
            return Path.Combine(DatabaseStatics.DataPath,$"{databaseName}.db3");
        }

        internal string[] SqlDatatypesTranslation()
        {
            return new[] { "varchar", "char", "int2", "integer", "bigint", "decimal", "float", "numeric", "varchar(36)", "boolean", "datetime", "blob" };
        }
    }
}
