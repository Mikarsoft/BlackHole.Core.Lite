
namespace BlackHole.Lite.Entities
{
    public abstract class BHEntity<T> : BHEntity where T : BHEntity<T>
    {
        public abstract void Congifure(RelationBuilder<T> modelBuilder);

    }

    public abstract class BHEntity
    {
        public int Id { get; set; }

        public int Inactive { get; internal set; }
    }
}
