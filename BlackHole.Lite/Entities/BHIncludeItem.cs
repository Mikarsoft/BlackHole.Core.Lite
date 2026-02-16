using BlackHole.Entities;

namespace BlackHole.Lite.Entities
{
    public struct BHIncludeItem<T> where T : BHEntity
    {
        public T? Value { get; set; }
    }
}
