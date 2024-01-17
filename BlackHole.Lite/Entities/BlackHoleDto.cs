
namespace BlackHole.Entities
{
    /// <summary>
    /// A Data transfer object, that 
    /// helps to map multiple tables on it,
    /// when using Joins Functionality
    /// </summary>
    public abstract class BlackHoleDto
    {
        /// <summary>
        /// The Primary Key of the DTO
        /// </summary>
        public int Id { get; set; }
    }
}
