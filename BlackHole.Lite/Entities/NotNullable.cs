
namespace BlackHole.Entities
{
    /// <summary>
    /// Attribute to enforce NOT NULL constraint on a property column in the database.
    /// Overrides the default nullable behavior for reference types.
    /// </summary>
    /// <remarks>
    /// By default, reference type properties (except strings marked [VarCharSize]) are nullable.
    /// Apply this attribute to a reference property to make the corresponding column NOT NULL.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class Order : BHEntity&lt;Order&gt;
    /// {
    ///     [NotNullable]
    ///     public string OrderNumber { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotNullable : Attribute
    {
        /// <summary>
        /// Indicates that the column is not nullable. Always false (property is read-only).
        /// </summary>
        public bool Nullability { get; } = false;
    }
}
