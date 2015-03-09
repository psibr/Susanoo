using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Transactions;

namespace Susanoo.UnitOfWork
{
    /// <summary>
    /// Class UnitOfWork.
    /// </summary>
    /// <typeparam name="TInfo">The type of the unit of work information.</typeparam>
    public abstract class UnitOfWork<TInfo> 
        : IRepository<TInfo>, IDisposable
    {
        private readonly List<ManagerInstance> _databaseManagers = new List<ManagerInstance>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="info">The context specific information.</param>
        /// <param name="managerMode">The connection mode.</param>
        protected UnitOfWork(TInfo info, UnitOfWorkDbManagerMode managerMode = UnitOfWorkDbManagerMode.PerTransaction)
            : this(managerMode)
        {
            Info = info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TInfo}"/> class.
        /// </summary>
        /// <param name="managerMode">The connection mode.</param>
        protected UnitOfWork(UnitOfWorkDbManagerMode managerMode)
        {
            ManagerMode = managerMode;
        }

        /// <summary>
        /// Gets the context specific information.
        /// </summary>
        /// <value>The information.</value>
        public TInfo Info { get; private set; }

        /// <summary>
        /// Gets the root context regardless of position in the context tree.
        /// </summary>
        UnitOfWork<TInfo> IRepository<TInfo>.UnitOfWork { get { return this; } }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        public DatabaseManager DatabaseManager
        {
            get
            {
                DatabaseManager result = null;

                switch (ManagerMode)
                {
                    case UnitOfWorkDbManagerMode.Shared:
                        if (_databaseManagers.Count == 0)
                        {
                            result = new DatabaseManager(ConnectionStringName);
                            result.OpenConnection(); // Creating Shared open connection for life of context.
                            _databaseManagers.Add(new ManagerInstance { DatabaseManager = result });
                        }
                        break;
                    case UnitOfWorkDbManagerMode.PerTransaction:
                        result = RetrieveOrAddDatabaseManagerPerTransaction(Transaction.Current);
                        break;
                    case UnitOfWorkDbManagerMode.PerCallOrTransaction:
                        if (Transaction.Current == null)
                        {
                            result = new DatabaseManager(ConnectionStringName);
                            _databaseManagers.Add(new ManagerInstance { DatabaseManager = result });
                        }
                        else
                        {
                            result = RetrieveOrAddDatabaseManagerPerTransaction(Transaction.Current);
                        }
                        break;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        protected abstract string ConnectionStringName { get; }

        /// <summary>
        /// Gets the connection mode.
        /// </summary>
        /// <value>The connection mode.</value>
        protected UnitOfWorkDbManagerMode ManagerMode { get; private set; }

        /// <summary>
        /// Retrieves the or add database manager per transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>DatabaseManager.</returns>
        private DatabaseManager RetrieveOrAddDatabaseManagerPerTransaction(Transaction transaction)
        {
            string transactionId = null;

            if (transaction != null)
                transactionId = transaction.TransactionInformation.LocalIdentifier;

            var container = _databaseManagers.FirstOrDefault((manager) => manager.TransactionId == transactionId);

            var result = (container != null) ? container.DatabaseManager : null;

            if (result == null)
            {
                result = new DatabaseManager(ConnectionStringName);

                result.OpenConnection(); // Creating Shared open connection for life of transaction.

                _databaseManagers.Add(new ManagerInstance { DatabaseManager = result, TransactionId = transactionId });
            }

            return result;
        }

        #region Resource Management

        private bool _disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="Context"/> class.
        /// </summary>
        ~UnitOfWork()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes
        /// </summary>
        /// <param name="disposing">is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var container in _databaseManagers.Where(container => container != null && container.DatabaseManager != null))
                        container.DatabaseManager.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion Resource Management
    }
}