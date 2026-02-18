
namespace BlackHole.Internal
{
    internal class TableCompleteInfo
    {
        internal TableCompleteInfo(Type tableType)
        {
            TableType = tableType;
        }

        internal Type TableType { get; set; }

        internal List<ColumnInfo> Columns = new();

        internal List<FKInfo> ForeignKeys = new();

        internal List<UniqueInfo> UniqueIndices = new();

        internal List<IndexInfo> Indices = new();
    }
}
