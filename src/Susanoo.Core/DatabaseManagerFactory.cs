using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// A factory that can build database managers from connections and connection strings.
    /// </summary>
    public class DatabaseManagerFactory 
        : IDatabaseManagerFactory
    {
        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public IDatabaseManager CreateFromConnectionStringName(string connectionStringName)
        {
            return DatabaseManager.CreateFromConnectionStringName(connectionStringName);
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>DatabaseManager.</returns>
        public IDatabaseManager CreateFromConnection(DbConnection connection)
        {
            return new DatabaseManager(connection);
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        public IDatabaseManager CreateFromConnectionStringName(DbProviderFactory provider, string connectionStringName)
        {
            return DatabaseManager.CreateFromConnectionStringName(provider, connectionStringName);
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public IDatabaseManager CreateFromConnectionStringName(string connectionStringName, string providerName)
        {
            return DatabaseManager.CreateFromConnectionStringName(connectionStringName, providerName);
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public IDatabaseManager CreateFromConnectionString(string connectionString, string providerName)
        {
            return DatabaseManager.CreateFromConnectionString(connectionString, providerName);
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public IDatabaseManager CreateFromConnectionString(DbProviderFactory provider, string connectionString)
        {
            return DatabaseManager.CreateFromConnectionString(provider, connectionString);
        }
    }
}
