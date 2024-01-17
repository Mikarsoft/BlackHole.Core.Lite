
namespace BlackHole.Entities
{
    /// <summary>
    /// Black Hole Entity. The table in database is based on this
    /// </summary>
    public abstract class BlackHoleEntity
    {
        /// <summary>
        /// The Primary Key of the Entity
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        internal int Inactive { get; set; }
    }
}
