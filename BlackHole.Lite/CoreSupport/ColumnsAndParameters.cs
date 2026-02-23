
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
        internal Type TableType { get; set; } = null!;

        internal string ForeignKeyProperty { get; set; } = null!;

        internal Type? ParentTableType { get; set; }

        internal string NavigationPropertyName { get; set; } = null!;

        internal bool IsReversed { get; set; }

        internal bool IsList { get; set; }

        internal int ParentIndex { get; set; }
    }
}
