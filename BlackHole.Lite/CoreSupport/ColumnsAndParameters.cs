
namespace BlackHole.CoreSupport
{
    internal class ColumnsAndParameters
    {
        internal string Columns { get; set; } = "";
        internal List<BlackHoleParameter> Parameters { get; set; } = new List<BlackHoleParameter>();
        internal int Count { get; set; }
    }

    internal class ColumnAndParameter
    {
        internal string? Column { get; set; }
        internal string? ParamName { get; set; }
        internal object? Value { get; set; }
    }

    internal class IncludeQueryCommand
    {
        internal string Query { get; set; } = string.Empty;

        internal string Joins { get; set; } = string.Empty;

        internal string RootLetter { get; set; } = string.Empty;
    }

    internal class IncludePart
    {
        public Type TableType { get; set; } = null!;

        public string ForeignKeyProperty { get; set; } = null!;

        public Type? ParentTableType { get; set; }

        public string NavigationPropertyName { get; set; } = null!;

        public bool IsReversed { get; set; }

        public bool IsList { get; set; }

        public int ParentIndex { get; set; }
    }
}
