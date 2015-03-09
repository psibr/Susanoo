namespace Susanoo.UnitOfWork
{
    /// <summary>
    /// Enum ContextDbConnectionMode
    /// </summary>
    public enum UnitOfWorkDbManagerMode
    {
        /// <summary>
        /// Uses a single database manager and connection for all calls (synchronous)
        /// </summary>
        Shared = 0,

        /// <summary>
        /// Maintains a shared database manager for most calls, but creates a new database manager for transaction calls.
        /// </summary>
        PerTransaction = 1,

        /// <summary>
        /// Creates a new database manager and connection for each call unless wrapped in a transaction.
        /// </summary>
        PerCallOrTransaction = 2
    }
}