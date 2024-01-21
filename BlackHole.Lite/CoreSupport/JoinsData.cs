
namespace BlackHole.CoreSupport
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class JoinsData<TSource, TOther>
    {
        internal string DatabaseName { get; set; } = string.Empty;
        internal Type? BaseTable { get; set; }
        internal List<TableLetters> TablesToLetters { get; set; } = new();
        internal List<TableProperties> AllProps { get; set; } = new();
        internal List<string?> Letters { get; set; } = new();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new();
        internal int HelperIndex { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class PreJoinsData<TSource, TOther>
    {
        internal string DatabaseName { get; set; } = string.Empty;

        internal Type? BaseTable { get; set; }
        internal List<TableLetters> TablesToLetters { get; set; } = new();
        internal List<TableProperties> AllProps { get; set; } = new();
        internal List<string?> Letters { get; set; } = new();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new();
        internal int HelperIndex { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
        internal bool IsFirstJoin { get; set; } = true;
        internal string? JoinType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JoinsData
    {
        internal string DatabaseName { get; set; } = string.Empty;

        internal Type? BaseTable { get; set; }
        internal List<TableLetters> TablesToLetters { get; set; } = new();
        internal List<TableProperties> AllProps { get; set; } = new();
        internal List<string?> Letters { get; set; } = new();
        internal string Joins { get; set; } = string.Empty;
        internal string WherePredicates { get; set; } = string.Empty;
        internal List<BlackHoleParameter> DynamicParams { get; set; } = new();
        internal int HelperIndex { get; set; }
        internal bool Ignore { get; set; }
        internal int ParamsCount { get; set; }
    }

    internal class TableLetters
    {
        internal Type? Table { get; set; }
        internal string? Letter { get; set; }
    }

    internal class TableProperties
    {
        internal string? PropName { get; set; }
        internal Type? PropType { get; set; }
        internal Type? TableType { get; set; }
        internal string? TableLetter { get; set; }
        internal bool HasPriority { get; set; }
    }

    internal class PropertyOccupation
    {
        internal string? PropName { get; set; }
        internal Type? PropType { get; set; }
        internal bool Occupied { get; set; }
        internal string? TableLetter { get; set; }
    }
}
