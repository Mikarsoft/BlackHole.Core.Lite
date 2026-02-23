

namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHIncludeList<T> where T : BHEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            Items.Add(item);
        }
    }
}
