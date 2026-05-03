using Microsoft.Data.Sqlite;

namespace BlackHole.CoreSupport
{
    /// <summary>
    /// Low-level SQLite transaction wrapper that manages database connections and transaction state.
    /// This class is typically obtained via <c>BHTransaction</c> (the public API) and should be used within a <c>using</c> statement
    /// to ensure proper cleanup of connections and resources.
    /// </summary>
    /// <remarks>
    /// Instances of BlackHoleTransaction are created by the ORM framework and provide access to the underlying
    /// <see cref="SqliteConnection"/> and <see cref="SqliteTransaction"/> objects. The transaction is automatically
    /// committed or rolled back when the instance is disposed, depending on whether errors occurred during the transaction lifetime.
    ///
    /// Typical usage pattern:
    /// <code>
    /// using (var transaction = new BlackHoleTransaction(connectionString))
    /// {
    ///     // Perform database operations
    ///     // If no errors occur, transaction commits on Dispose
    ///     // If hasError is set to true, transaction rolls back instead
    /// }
    /// </code>
    /// </remarks>
    public class BlackHoleTransaction : IDisposable
    {
        /// <summary>
        /// The underlying SQLite connection object. Access this to execute raw SQL commands or inspect connection state.
        /// </summary>
        public SqliteConnection connection;

        /// <summary>
        /// The underlying SQLite transaction object. Use this for advanced transaction control if needed,
        /// though most users should rely on the Dispose behavior for automatic commit/rollback.
        /// </summary>
        public SqliteTransaction _transaction;

        private bool commited = false;

        /// <summary>
        /// Set to true by the ORM if an error occurs during transaction operations. When Dispose is called,
        /// if this flag is true, the transaction is rolled back; otherwise it is committed.
        /// </summary>
        internal bool hasError = false;

        private bool pendingRollback = false;

        /// <summary>
        /// Initializes a new transaction for the given connection string.
        /// </summary>
        /// <param name="connectionString">The SQLite connection string. If invalid, falls back to the default connection.</param>
        /// <remarks>
        /// This constructor is typically called internally by the ORM and not by end users.
        /// </remarks>
        internal BlackHoleTransaction(string connectionString)
        {
            try
            {
                connection = BHDataProviderSelector.GetConnection(connectionString);
                connection.Open();
                _transaction = connection.BeginTransaction();
            }
            catch
            {
                connection = BHDataProviderSelector.GetConnection();
                connection.Open();
                _transaction = connection.BeginTransaction();
            }
        }

        internal bool Commit()
        {
            if (!commited)
            {
                commited = true;

                if (hasError)
                {
                    _transaction.Rollback();
                    return false;
                }

                _transaction.Commit();
                return commited;
            }
            return false;
        }

        internal bool DoNotCommit()
        {
            if (!commited)
            {
                commited = true;
                pendingRollback = true;
                return true;
            }

            return false;
        }

        internal bool RollBack()
        {
            if (!commited || pendingRollback)
            {
                _transaction.Rollback();
                hasError = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finalizes and cleans up the transaction and connection resources.
        /// If the transaction has not yet been explicitly committed, this method will commit or rollback based on error state,
        /// then dispose the transaction and close/dispose the connection.
        /// </summary>
        /// <remarks>
        /// The behavior is as follows:
        /// - If <see cref="hasError"/> is true, the transaction is rolled back.
        /// - Otherwise, the transaction is committed.
        /// - If a pending rollback is flagged, the rollback takes precedence.
        /// - Both the transaction and connection are always disposed to free resources.
        ///
        /// This method should be called automatically when the instance is used in a <c>using</c> statement.
        /// </remarks>
        public void Dispose()
        {
            if (!commited)
            {
                if (hasError)
                {
                    _transaction.Rollback();
                }
                else
                {
                    _transaction.Commit();
                }
            }

            if (pendingRollback)
            {
                _transaction.Rollback();
            }

            _transaction.Dispose();
            connection.Close();
            connection.Dispose();
        }
    }
}
