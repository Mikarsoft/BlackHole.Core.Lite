

namespace BlackHole.Internal
{
    /// <summary>
    /// Represents a foreign key constraint from a SQLite PRAGMA foreign_key_list result.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It mirrors
    /// the output of SQLite's PRAGMA foreign_key_list(table_name).
    /// </remarks>
    public class SQLiteForeignKeySchema
    {
        /// <summary>
        /// Gets or sets the foreign key id (ordinal position within the table's foreign keys).
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Gets or sets the sequence number of the column pair in a composite foreign key.
        /// </summary>
        public int seq { get; set; }
        /// <summary>
        /// Gets or sets the name of the referenced foreign table.
        /// </summary>
        public string table { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the column in the current (child) table.
        /// </summary>
        public string from { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name of the column in the referenced (parent) table.
        /// </summary>
        public string to { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the action taken when a referenced row is updated (e.g., "SET NULL", "CASCADE", "RESTRICT").
        /// </summary>
        public string on_update { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the action taken when a referenced row is deleted (e.g., "SET NULL", "CASCADE", "RESTRICT").
        /// </summary>
        public string on_delete { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the match type for the foreign key constraint (e.g., "SIMPLE", "FULL", "PARTIAL").
        /// </summary>
        public string match { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents autoincrement sequence information for a SQLite table.
    /// </summary>
    /// <remarks>
    /// This type is an internal implementation detail exposed for schema introspection during table
    /// build and alter operations. It is not intended for direct use in end-user code. It holds
    /// metadata from SQLite's sqlite_sequence table regarding autoincrement sequences.
    /// </remarks>
    public class LiteAutoIncrementInfo
    {
        /// <summary>
        /// Gets or sets the name of the table with the autoincrement column.
        /// </summary>
        public string name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the last autoincrement sequence value assigned.
        /// </summary>
        public int seq { get; set; }
    }
}
