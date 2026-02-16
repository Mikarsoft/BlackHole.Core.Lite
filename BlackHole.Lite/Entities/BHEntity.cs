
namespace BlackHole.Lite.Entities
{
    public abstract class BHEntity<T> : BHEntity where T : BHEntity<T>
    {
        public abstract void Congifure(RelationBuilder<T> modelBuilder);

    }

    public abstract class BHEntity
    {
        public  abstract int Id { get; set; }
    }
}
