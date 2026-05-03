
namespace BlackHole.Entities
{
    /// <summary>
    /// Marks a navigation property for a one-to-one relationship between entities.
    /// Used on entity properties to declare a single related entity.
    /// </summary>
    /// <typeparam name="T">The type of the related entity. Must derive from <see cref="BHEntity"/>.</typeparam>
    /// <remarks>
    /// This is a marker type used within the fluent configuration API (via <see cref="RelationBuilder{T}.HasOne{G}(Expression{Func{T, BHIncludeItem{G}}})"/>).
    /// Unlike <see cref="BHIncludeList{T}"/>, this represents a single entity rather than a collection.
    /// Navigation properties using this type are not stored in the database; they are lazy-loaded or included via the Include mechanism.
    /// </remarks>
    public class BHIncludeItem<T> where T : BHEntity
    {
        /// <summary>
        /// The related entity instance, or null if no related entity exists.
        /// </summary>
        public T? Value { get; set; }
    }
}
