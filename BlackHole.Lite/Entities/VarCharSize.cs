namespace BlackHole.Entities
{
    /// <summary>
    /// Attribute to specify the maximum character length of a VARCHAR column in the database.
    /// Default size is 255 if not specified.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to string properties to control the VARCHAR(n) column size in SQLite.
    /// This affects both the database schema and validation at the ORM level.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class User : BHEntity&lt;User&gt;
    /// {
    ///     [VarCharSize(100)]
    ///     public string FirstName { get; set; }
    ///
    ///     [VarCharSize(500)]
    ///     public string Bio { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VarCharSize : Attribute
    {
        /// <summary>
        /// The maximum character length of the VARCHAR column.
        /// </summary>
        public int Charlength { get; set; }

        /// <summary>
        /// Initializes a new instance specifying the VARCHAR column size.
        /// </summary>
        /// <param name="Characters">The number of characters for VARCHAR(n).</param>
        public VarCharSize(int Characters)
        {
            Charlength = Characters;
        }
    }
}
