namespace BlackHole.Core
{
    /// <summary>
    /// An interface that run on the startup of the Application
    /// and stores Joins as Views, to be used by the BHDataProvider 
    /// at any point.
    /// </summary>
    public interface IBHInitialViews
    {
        /// <summary>
        /// An interface that run on the startup of the Application
        /// and stores Joins as Views, to be used by the BHDataProvider 
        /// at any point.
        /// </summary>
        /// <param name="initializer">A simple Data Provider that can only create Joins</param>
        public void DefaultViews(BHViewInitializer initializer);
    }
}
