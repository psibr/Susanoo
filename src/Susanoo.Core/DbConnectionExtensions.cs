using System.Data.Common;

namespace Susanoo
{
    /// <summary>
    /// Provides extensions on the DbConnection
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Uses the connection object to build a database manager.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>IDatabaseManager.</returns>
        public static IDatabaseManager ToDatabaseManager(this DbConnection connection)
        {
            return CommandManager.Instance.Bootstrapper
                .ResolveDatabaseManagerFactory()
                .CreateFromConnection(connection);
        }
    }
}
