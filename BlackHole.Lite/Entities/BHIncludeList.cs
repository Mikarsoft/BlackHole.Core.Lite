

namespace BlackHole.Entities
{
    /// <summary>
    /// Marks a navigation property for a one-to-many relationship between entities.
    /// Used on entity properties to declare a collection of related entities.
    /// </summary>
    /// <typeparam name="T">The type of entities in the collection. Must derive from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// This is a marker type used within the fluent configuration API (via <see cref="RelationBuilder{T}.HasOne{G}(Expression{Func{T, BHIncludeItem{G}}})"/>
    /// and <see cref="BHIncludeKey{T, G}.WithMany(Expression{Func{G, BHIncludeList{T}}})"/>).
    /// Unlike <see cref="BHIncludeItem{T}"/>, this represents a collection of related entities.
    /// Navigation properties using this type are not stored in the database; they are lazy-loaded or included via the Include mechanism.
    /// </remarks>
    public class BHIncludeList<T> where T : BHEntity
    {
        /// <summary>
        /// The collection of related entities.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Adds a single entity to the collection.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public void Add(T item)
        {
            Items.Add(item);
        }
    }
}
