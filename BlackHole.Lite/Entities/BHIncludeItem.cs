
namespace BlackHole.Entities
{
    public struct BHIncludeItem<T> where T : BHEntity
    {
        public T? Value { get; set; }
    }
}
