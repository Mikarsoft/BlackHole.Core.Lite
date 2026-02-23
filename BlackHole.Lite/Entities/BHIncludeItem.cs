
namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHIncludeItem<T> where T : BHEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public T? Value { get; set; }
    }
}
