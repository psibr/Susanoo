using System.Data.Common;

namespace Susanoo
{
    /// <summary>
    /// A factory that can build database managers.
    /// </summary>
    public interface IDatabaseManagerFactory
    {
        /// <summary>
        /// Creates a DatabaseManager from a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>DatabaseManager.</returns>
        IDatabaseManager CreateFromConnection(DbConnection connection);

#if !DOTNETCORE
        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        IDatabaseManager CreateFromConnectionStringName(string connectionStringName);

        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        IDatabaseManager CreateFromConnectionStringName(DbProviderFactory provider, string connectionStringName);


        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        IDatabaseManager CreateFromConnectionStringName(string connectionStringName, string providerName);

        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        IDatabaseManager CreateFromConnectionString(string connectionString, string providerName);
#endif
        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        IDatabaseManager CreateFromConnectionString(DbProviderFactory provider, string connectionString);
    }
}
