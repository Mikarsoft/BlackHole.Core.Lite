namespace BlackHole.Internal
{
    /// <summary>
    /// Represents column information from a SQLite PRAGMA table_info result.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It mirrors
    /// the output of SQLite's PRAGMA table_info(table_name).
    /// </remarks>
    public class SQLiteTableInfo
    {
        /// <summary>
        /// Gets or sets the column id (ordinal position in table definition).
        /// </summary>
        public int cid { get; set; }
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the declared type of the column (e.g., "INTEGER", "TEXT", "REAL").
        /// </summary>
        public string type { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets a value indicating whether the column has a NOT NULL constraint.
        /// </summary>
        public bool notnull { get; set; }
        /// <summary>
        /// Gets or sets the default value expression for the column, or empty string if none.
        /// </summary>
        public string dflt_value { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the primary key position (1-based, or 0 if not part of primary key).
        /// </summary>
        public int pk { get; set; }
    }

    /// <summary>
    /// Represents index metadata from a SQLite PRAGMA index_list result.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It mirrors
    /// the output of SQLite's PRAGMA index_list(table_name).
    /// </remarks>
    public class SQLiteIndexInfo
    {
        /// <summary>
        /// Gets or sets the sequence number of the index.
        /// </summary>
        public int seq { get; set; }

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the index enforces uniqueness.
        /// </summary>
        public bool unique { get; set; }

        /// <summary>
        /// Gets or sets the origin of the index: "c" for CREATE INDEX, "u" for UNIQUE constraint, "pk" for primary key.
        /// </summary>
        public string origin { get; set; } = string.Empty; // "c" = CREATE INDEX, "u" = UNiQUE CONSTRAINT, "pk" = primary key

        /// <summary>
        /// Gets or sets a value indicating whether the index is partial (has a WHERE clause).
        /// </summary>
        public bool partial { get; set; }
    }

    /// <summary>
    /// Represents column information for an index from a SQLite PRAGMA index_xinfo result.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It mirrors
    /// the output of SQLite's PRAGMA index_xinfo(index_name).
    /// </remarks>
    public class SQLiteIndexColumnInfo
    {
        /// <summary>
        /// Gets or sets the sequence number of the column within the index.
        /// </summary>
        public int seqno { get; set; }
        /// <summary>
        /// Gets or sets the column id (ordinal position) from the table, or -1 for computed expressions.
        /// </summary>
        public int cid { get; set; }
        /// <summary>
        /// Gets or sets the name of the column in the index.
        /// </summary>
        public string name { get; set; } = string.Empty;
    }
}
