namespace BlackHole.Core
{
    /// <summary>
    /// An interface that can be used , to  insert default data
    ///  in a newly created database.
    /// </summary>
    public interface IBHInitialData
    {
        /// <summary>
        /// A contact method that allows to insert default data 
        /// in a newly created database
        /// </summary>
        /// <param name="initializer">A data provider that accepts custom sql commands or sql files</param>
        public void DefaultData(BHDataInitializer initializer);
    }
}
