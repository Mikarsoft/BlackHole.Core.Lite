using BlackHole.CoreSupport;
using BlackHole.Entities;

namespace BlackHole.Core
{
    /// <summary>
    /// Encapsulates a database transaction, automating connection and rollback management.
    /// Implement with <c>using</c> to ensure proper cleanup.
    /// </summary>
    /// <remarks>
    /// If no explicit action is taken (no Commit or RollBack call), the transaction
    /// auto-commits on Dispose. Pass a BHTransaction to any CRUD method to enlist it
    /// in that operation. Soft-delete behavior with [UseActivator] applies within transactions.
    /// </remarks>
    /// <example>
    /// <code>
    /// using var transaction = new BHTransaction();
    /// try
    /// {
    ///     var userId = BHDataProvider.For&lt;User&gt;()
    ///         .InsertEntry(new User { Name = "Bob" }, transaction);
    ///     var orderId = BHDataProvider.For&lt;Order&gt;()
    ///         .InsertEntry(new Order { UserId = userId, Amount = 99.99m }, transaction);
    ///     transaction.Commit();
    /// }
    /// catch
    /// {
    ///     transaction.RollBack();
    ///     throw;
    /// }
    /// </code>
    /// </example>
    public class BHTransaction : IDisposable
    {
        internal BlackHoleTransaction transaction;
        internal string DBName { get; set; } 

        /// <summary>
        /// A Transaction Object that automatically creates a connection and a transaction
        /// and it can be used in every BlackHole Method.
        /// <para>Tip: Don't forget to dispose this Object after using it. If you don't perform any 
        /// action on this class, the Commit Method gets triggered on Dispose</para>
        /// </summary>
        public BHTransaction()
        {
            DBName = BHDataProviderSelector.GetDefaultDbName();
            transaction = new BlackHoleTransaction(DBName.BuildConnectionString());
        }

        /// <summary>
        /// A Transaction Object that automatically creates a connection and a transaction
        /// on specific database file
        /// and it can be used in every BlackHole Method.
        /// <para>Tip: Don't forget to dispose this Object after using it. If you don't perform any 
        /// action on this class, the Commit Method gets triggered on Dispose</para>
        /// </summary>
        /// <param name="databaseName"></param>
        public BHTransaction(string databaseName)
        {
            DBName = databaseName;
            transaction = new BlackHoleTransaction(DBName.BuildConnectionString());
        }

        /// <summary>
        /// Gets the entity context for the specified entity type within this transaction.
        /// </summary>
        /// <typeparam name="T">An entity class deriving from <see cref="BHEntity"/>.</typeparam>
        /// <remarks>
        /// All operations on the returned context will execute within this transaction's scope.
        /// </remarks>
        /// <returns>A transactional entity context for executing queries within this transaction.</returns>
        public BHTransactEntityContext<T> For<T>() where T : BHEntity
        {
            return BHDataProvider.For<T>().MapEntityTransact(transaction);
        }

        /// <summary>
        /// Commits all changes made within this transaction, persisting them to the database.
        /// </summary>
        /// <remarks>
        /// After committing, the transaction is no longer active. Do not attempt further
        /// database operations with this transaction; create a new one if needed.
        /// </remarks>
        /// <returns>True if the commit succeeded; false otherwise.</returns>
        public bool Commit()
        {
            return transaction.Commit();
        }

        /// <summary>
        /// Marks the transaction to be rolled back instead of committed when disposed.
        /// </summary>
        /// <remarks>
        /// Calling this prevents automatic commit on Dispose. The transaction will
        /// be explicitly rolled back when disposed.
        /// </remarks>
        /// <returns>True if the operation succeeded; false otherwise.</returns>
        public bool DoNotCommit()
        {
            return transaction.DoNotCommit();
        }

        /// <summary>
        /// Rolls back all changes made within this transaction, discarding any modifications.
        /// </summary>
        /// <remarks>
        /// After rolling back, the transaction is no longer active. All data modifications
        /// are discarded and the database is left unchanged.
        /// </remarks>
        /// <returns>True if the rollback succeeded; false otherwise.</returns>
        public bool RollBack()
        {
            return transaction.RollBack();
        }

        /// <summary>
        /// Disposes the Connection and the transaction. If no other action have been used,
        /// it also Commits the transaction.
        /// </summary>
        public void Dispose()
        {
            transaction.Dispose();
        }
    }
}
