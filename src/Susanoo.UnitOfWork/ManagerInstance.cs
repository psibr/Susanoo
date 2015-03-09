namespace Susanoo.UnitOfWork
{
    /// <summary>
    /// Class ManagerInstance.
    /// </summary>
    internal class ManagerInstance
    {
        /// <summary>
        /// Gets or sets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        public DatabaseManager DatabaseManager { get; set; }

        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>The transaction identifier.</value>
        public string TransactionId { get; set; }
    }
}