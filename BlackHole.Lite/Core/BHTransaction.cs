﻿using BlackHole.CoreSupport;

namespace BlackHole.Core
{
    /// <summary>
    /// A Transaction Object that automatically creates a connection and a transaction
    /// and it can be used in every BlackHole Method.
    /// <para>Tip: Don't forget to dispose this Object after using it. If you don't perform any 
    /// action on this class, the Commit Method gets triggered on Dispose</para>
    /// </summary>
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

        public BHTransaction(string databaseName)
        {
            DBName = databaseName;
            transaction = new BlackHoleTransaction(DBName.BuildConnectionString());
        }

        /// <summary>
        /// Commit the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            return transaction.Commit();
        }

        /// <summary>
        /// Block the transaction.
        /// </summary>
        /// <returns></returns>
        public bool DoNotCommit()
        {
            return transaction.DoNotCommit();
        }

        /// <summary>
        /// RollBack the transaction
        /// </summary>
        /// <returns></returns>
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
