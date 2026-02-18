

namespace BlackHole.Internal
{
    internal class ColumnInfo
    {
        public Type PropertyType { get; set; } = null!;

        public Type PropertyBaseType { get; set; } = null!;

        public string PropertyName { get; set; } = string.Empty;

        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsCompositeKey { get; set; }

        public int Size { get; set; }
    }
}
