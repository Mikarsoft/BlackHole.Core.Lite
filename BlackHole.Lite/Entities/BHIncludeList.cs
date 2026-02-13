

using BlackHole.Entities;

namespace BlackHole.Lite.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct BHIncludeList<T> where T : BlackHoleEntity
    {
        public List<T> Items { get; set; }

        public void Add(T item)
        {
            Items ??= new List<T>();
            Items.Add(item);
        }
    }
}
