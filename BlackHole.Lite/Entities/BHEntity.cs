
namespace BlackHole.Entities
{
    /// <summary>
    /// Base class for all BlackHole entities that supports fluent relationship configuration.
    /// Derives from <see cref="BHEntity"/> and provides an entry point for defining relationships via the <see cref="Congifure(RelationBuilder{T})"/> method.
    /// </summary>
    /// <typeparam name="T">The entity type inheriting from this class. Must implement BHEntity&lt;T&gt;.</typeparam>
    /// <remarks>
    /// The <see cref="Congifure(RelationBuilder{T})"/> method is called automatically during database initialization
    /// to configure navigation properties, foreign keys, and indexes using a fluent API similar to Entity Framework.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class User : BHEntity&lt;User&gt;
    /// {
    ///     public string Name { get; set; }
    ///     public BHIncludeList&lt;Order&gt; Orders { get; set; }
    ///
    ///     public override void Congifure(RelationBuilder&lt;User&gt; modelBuilder)
    ///     {
    ///         modelBuilder.HasOne(x => x.Orders)
    ///             .WithMany(x => x.User);
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class BHEntity<T> : BHEntity where T : BHEntity<T>
    {
        /// <summary>
        /// Configures relationships, foreign keys, and indexes for this entity.
        /// This method is invoked automatically during database table initialization.
        /// Note: The method name contains a typo and is spelled "Congifure" in the API.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="RelationBuilder{T}"/> used to configure relationships and constraints.</param>
        /// <remarks>
        /// Use this method to declare one-to-one and one-to-many relationships, set foreign key behavior (cascade, restrict, set null),
        /// and configure indexes on entity properties.
        /// </remarks>
        public abstract void Congifure(RelationBuilder<T> modelBuilder);

    }

    /// <summary>
    /// Base class for all BlackHole entities.
    /// Provides the primary key and soft-delete flag used when <see cref="UseActivator"/> is applied to an entity.
    /// </summary>
    /// <remarks>
    /// Every entity automatically inherits an <see cref="Id"/> primary key (auto-incrementing integer).
    /// The <see cref="Inactive"/> flag is used for soft-delete behavior when the entity is decorated with <see cref="UseActivator"/>;
    /// setting Inactive=1 marks a row as deleted without removing it from the database.
    /// </remarks>
    public abstract class BHEntity
    {
        /// <summary>
        /// The primary key of the entity. Auto-incremented integer set by the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Internal soft-delete flag. Set to 1 when the entity is marked inactive; used only when <see cref="UseActivator"/> is applied to the entity class.
        /// Inactive entities are excluded from normal queries and can only be accessed via specialized methods.
        /// </summary>
        public int Inactive { get; internal set; }
    }
}
