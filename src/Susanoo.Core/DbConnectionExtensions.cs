using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .ResolveDependency<IDatabaseManagerFactory>()
                .CreateFromConnection(connection);
        }
    }
}
