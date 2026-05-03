
namespace BlackHole.CoreSupport
{
    /// <summary>
    /// Represents the state of a completed join operation between a source entity type and another entity type.
    /// This class stores the SQL join structure, where predicates, parameters, and table aliases needed to execute
    /// fluent join queries such as <c>.InnerJoin().On().Where().Cast&lt;DTO&gt;().SelectAll()</c>.
    /// Instances are created internally by the ORM join fluent API and are typically not instantiated directly by users.
    /// </summary>
    /// <typeparam name="TSource">The primary (left) entity type in the join.</typeparam>
    /// <typeparam name="TOther">The secondary (right) entity type being joined to the primary.</typeparam>
    /// <remarks>
    /// This class is used internally to accumulate join and filter state as the user chains fluent methods.
    /// The state is then used by the ORM to generate and execute the appropriate SQL query.
    /// </remarks>
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
    /// Represents the initial state of a join operation, before the join condition (ON clause) is specified.
    /// This class is returned by join initiator methods like <c>BHDataProvider.InnerJoin&lt;T1, T2&gt;()</c>
    /// and provides the fluent API entry point for defining the join condition and filters.
    /// </summary>
    /// <typeparam name="TSource">The primary (left) entity type in the join.</typeparam>
    /// <typeparam name="TOther">The secondary (right) entity type being joined to the primary.</typeparam>
    /// <remarks>
    /// Users interact with this class through fluent method calls like <c>.On(sourceKey, otherKey).Where(...)</c>.
    /// The class accumulates the join state and transitions to <see cref="JoinsData{TSource, TOther}"/> once
    /// the ON clause is specified.
    /// </remarks>
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
    /// Non-generic variant of join state that holds the accumulated SQL join structure and filter state.
    /// This class is used internally when join type information is not needed or when the ORM needs
    /// to work with join state generically across different entity type combinations.
    /// </summary>
    /// <remarks>
    /// This is a legacy or internal API exposed for completeness. Most users interact with the generic
    /// variants <see cref="JoinsData{TSource, TOther}"/> and <see cref="PreJoinsData{TSource, TOther}"/> instead.
    /// </remarks>
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
