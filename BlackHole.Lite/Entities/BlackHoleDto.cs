
namespace BlackHole.Entities
{
    /// <summary>
    /// Base class for view DTOs used in join queries and stored views.
    /// Represents a read-only projection of data from one or more tables.
    /// </summary>
    /// <remarks>
    /// Derived from BlackHoleDto, these classes are used as result shapes for queries executed via
    /// <c>BHDataProvider.ExecuteView&lt;TDto&gt;()</c> or custom joins. They provide a flattened
    /// representation of normalized database data, combining columns from multiple tables.
    /// </remarks>
    public abstract class BlackHoleDto
    {
        /// <summary>
        /// The primary key of the DTO, typically from the primary entity in the view or join result.
        /// </summary>
        public int Id { get; set; }
    }
}
