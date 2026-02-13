using BlackHole.Entities;

namespace BlackHole.Lite.Entities
{
    public struct BHIncludeItem<T> where T : BlackHoleEntity
    {
        public T? Value { get; set; }
    }
}
