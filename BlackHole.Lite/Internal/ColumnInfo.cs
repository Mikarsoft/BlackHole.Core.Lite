

namespace BlackHole.Lite.Internal
{
    internal class ColumnInfo
    {
        public string PropertyName { get; set; } = string.Empty;

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsCompositeKey { get; set; }
    }
}
