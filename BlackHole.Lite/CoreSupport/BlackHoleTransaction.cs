﻿using Microsoft.Data.Sqlite;

namespace BlackHole.CoreSupport
{
    /// <summary>
    /// Transaction Object
    /// </summary>
    public class BlackHoleTransaction : IDisposable
    {
        /// <summary>
        /// Generic connection
        /// </summary>
        public SqliteConnection connection;
        /// <summary>
        /// Generic transaction
        /// </summary>
        public SqliteTransaction _transaction;
        private bool commited = false;
        internal bool hasError = false;
        private bool pendingRollback = false;

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
        /// Commit uncommited transaction. Dispose the connection and the transaction
        /// </summary>
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
