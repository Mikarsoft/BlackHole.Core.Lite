
namespace BlackHole.Internal
{
    /// <summary>
    /// Represents intermediate schema parsing state for a table column, including metadata
    /// collected during table introspection and used to decide CREATE TABLE or ALTER TABLE statements.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It holds
    /// normalized column metadata across multiple database vendors and is consumed by BHTableBuilder
    /// to generate or update table schemas.
    /// </remarks>
    public class TableParsingInfo
    {
        /// <summary>
        /// Gets or sets the name of the table containing this column.
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the column being parsed.
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the column in the foreign key reference target.
        /// </summary>
        public string ReferencedColumn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default value expression for the column.
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets extra column attributes (e.g., autoincrement flags from MySQL).
        /// </summary>
        public string Extra { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the declared data type of the column (e.g., "INTEGER", "TEXT", "REAL").
        /// </summary>
        public string DataType { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the maximum character length of a string column, or 0 if unbounded.
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Gets or sets the numeric precision (total number of digits) for numeric types.
        /// </summary>
        public int NumPrecision { get; set; }
        /// <summary>
        /// Gets or sets the numeric scale (number of fractional digits) for decimal types.
        /// </summary>
        public int NumScale { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the column allows NULL values.
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the column is part of the primary key.
        /// </summary>
        public bool PrimaryKey { get; set; }
        /// <summary>
        /// Gets or sets the referential action for deletions on a foreign key constraint (e.g., "SET NULL", "CASCADE").
        /// </summary>
        public string DeleteRule { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the table referenced by a foreign key constraint.
        /// </summary>
        public string ReferencedTable { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the constraint (e.g., foreign key or index constraint name).
        /// </summary>
        public string ConstraintName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the column is an identity (autoincrement) column in SQL Server.
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
