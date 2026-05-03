namespace BlackHole.Entities
{
    /// <summary>
    /// Attribute to enable soft-delete (logical delete) for an entity.
    /// When applied to an entity class, deletes set the <see cref="BHEntity.Inactive"/> flag to 1 instead of removing the row from the database.
    /// </summary>
    /// <remarks>
    /// Soft-deleted entities (with Inactive=1) are automatically excluded from all normal queries and operations.
    /// They can only be accessed via specialized methods like GetAllInactiveEntries and DeleteInactiveEntryById.
    /// This approach preserves historical data while keeping it logically invisible in normal operations.
    /// The Inactive column must exist on the entity (inherited from BHEntity).
    /// </remarks>
    /// <example>
    /// <code>
    /// [UseActivator]
    /// public class Customer : BHEntity&lt;Customer&gt;
    /// {
    ///     public string Name { get; set; }
    ///
    ///     public override void Congifure(RelationBuilder&lt;Customer&gt; modelBuilder)
    ///     {
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UseActivator : Attribute
    {
        /// <summary>
        /// Indicates that the Inactive flag is used for soft-delete behavior. Always true.
        /// </summary>
        public bool useActivator = true;
    }
}
