#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif
using System.Transactions;

#endregion

namespace Susanoo
{
    /// <summary>
    /// Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public partial class DatabaseManager : IDatabaseManager, IDisposable
    {
        private DbConnection _connection;
        private bool _explicitlyOpened;
        private string _connectionString;
        private readonly Action<DbCommand> _providerSpecificCommandSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerSpecificCommandSettings">The provider specific CommandBuilder settings.</param>
        /// <exception cref="NotSupportedException">The database provider type specified is not supported. </exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName,
            Action<DbCommand> providerSpecificCommandSettings)
            : this(provider, connectionStringName)
        {
            _providerSpecificCommandSettings = providerSpecificCommandSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="NotSupportedException">The database provider type specified is not supported. </exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName)
        {
            Provider = provider;

            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="ArgumentException">Provider is a required component of the connection
        /// string.;connectionStringName</exception>
        public DatabaseManager(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            Provider =
                DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);

            if (Provider == null)
                throw new ArgumentException("Provider is a required component of the connection string.",
                    nameof(connectionStringName));
        }

        private DatabaseManager()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public DatabaseManager(DbConnection connection)
        {
            this._connection = connection;

            if (new[]
            {
                ConnectionState.Connecting,
                ConnectionState.Open,
                ConnectionState.Executing,
                ConnectionState.Fetching
            }.Contains(connection.State))
            {
                this._explicitlyOpened = true;
            }
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public static DatabaseManager CreateFromConnectionStringName(string connectionStringName)
        {
            var manager = new DatabaseManager();

            manager._connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            manager.Provider =
                DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);

            if (manager.Provider == null)
                throw new ArgumentException("Provider is a required component of the connection string.",
                    nameof(connectionStringName));

            return manager;
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>DatabaseManager.</returns>
        public static DatabaseManager CreateFromConnectionStringName(DbProviderFactory provider, string connectionStringName)
        {
            var manager = new DatabaseManager
            {
                Provider = provider,
                _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                    .ConnectionString
            };


            return manager;
        }


        /// <summary>
        /// Creates a DatabaseManager from a connection string name by resolving from configuration.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public static DatabaseManager CreateFromConnectionStringName(string connectionStringName, string providerName)
        {
            var manager = new DatabaseManager { Provider = DbProviderFactories.GetFactory(providerName) };

            if (manager.Provider == null)
                throw new ArgumentException("Provider is a required component of the connection string.",
                    nameof(providerName));

            manager._connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;

            return manager;
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public static DatabaseManager CreateFromConnectionString(string connectionString, string providerName)

        {
            var manager = new DatabaseManager
            {
                _connectionString = connectionString,
                Provider = DbProviderFactories.GetFactory(providerName)
            };

            if (manager.Provider == null)
                throw new ArgumentException("Provider is a required component of the connection string.",
                    nameof(providerName));

            return manager;
        }

        /// <summary>
        /// Creates a DatabaseManager from a connection string and providerName.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>DatabaseManager.</returns>
        /// <exception cref="ArgumentException">Provider is a required component of the connection string.</exception>
        public static DatabaseManager CreateFromConnectionString(DbProviderFactory provider, string connectionString)

        {
            var manager = new DatabaseManager
            {
                _connectionString = connectionString,
                Provider = provider
            };

            return manager;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        protected DbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = Provider.CreateConnection();
                    if (_connection != null) _connection.ConnectionString = _connectionString;
                }

                return _connection;
            }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public DbProviderFactory Provider { get; private set; }

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout"></param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType, TimeSpan commandTimeout,
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            IDataReader results = null;

            try
            {
                var open = _explicitlyOpened;
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {
                    // If the connection was open before execute was called, then do not automatically close connection.
                    results = open ? command.ExecuteReader() : command.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                if (results != null && !results.IsClosed)
                    results.Close();

                throw;
            }

            return results;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A single value of type T.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, TimeSpan commandTimeout, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            var open = _explicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {
                    var result = CastValue(typeof(T), command.ExecuteScalar());

                    return (T)result;
                }
            }
            finally
            {
                if (Transaction.Current == null && !open)
                    CloseConnection();
            }
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, TimeSpan commandTimeout, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            var open = (Connection.State != ConnectionState.Closed);

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (Transaction.Current == null && !open)
                    CloseConnection();
            }
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns>DbParameter.</returns>
        public virtual DbParameter CreateParameter()
        {
            return Provider?.CreateParameter() ?? Connection.CreateCommand().CreateParameter();
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>DbParameter.</returns>
        public virtual DbParameter CreateParameter(string parameterName, ParameterDirection parameterDirection,
            DbType parameterType, object value)
        {
            var newParam = Provider.CreateParameter();
            if (newParam != null)
            {
                newParam.ParameterName = parameterName;
                newParam.Direction = parameterDirection;
                newParam.DbType = parameterType;
                newParam.Value = value;
            }
            return newParam;
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>DbParameter.</returns>
        public virtual DbParameter CreateInputParameter(string parameterName, DbType parameterType, object value)
        {
            return CreateParameter(parameterName, ParameterDirection.Input, parameterType, value);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public virtual void OpenConnection()
        {
            _explicitlyOpened = true;

            OpenConnectionInternal();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public virtual void CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                _explicitlyOpened = false;
            }
        }

        /// <summary>
        /// Gets the state of the connection.
        /// </summary>
        /// <value>The state.</value>
        public ConnectionState State =>
            Connection.State;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs a bulk copy operation.
        /// </summary>
        /// <typeparam name="TRecord">The type of the record.</typeparam>
        /// <param name="destinationTableName">Name of the destination table.</param>
        /// <param name="records">The records.</param>
        /// <param name="whiteList">The white list of properties to include. Default is NULL.</param>
        /// <param name="blackList">The black list of properties to exclude. Default is NULL.</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void BulkCopy<TRecord>(string destinationTableName,
            IEnumerable<TRecord> records,
            IEnumerable<string> whiteList = null,
            IEnumerable<string> blackList = null)
        {
            //TODO: Implement BulkCopy operation.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Realizes an instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        ~DatabaseManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Returns value or it's string representation.
        /// </summary>
        /// <param name="newType">The new type.</param>
        /// <param name="value">The value.</param>
        /// <returns>Value or string representation.</returns>
#if !NETFX40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object CastValue(Type newType, object value)
        {
            if (value == DBNull.Value)
                value = null;

            var returnValue = value;

            if (newType == typeof(string))
            {
                returnValue = (value ?? "").ToString();
            }

            return returnValue;
        }

        /// <summary>
        /// Preps the CommandBuilder.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>DbCommand.</returns>
        protected virtual DbCommand PrepCommand(DbConnection connection, string commandText, CommandType commandType,
             TimeSpan commandTimeout, params DbParameter[] parameters)
        {
            var command = Connection.CreateCommand();

            command.CommandType = commandType;
            command.Connection = Connection;

            command.CommandText = commandText;

            command.CommandTimeout = Convert.ToInt32(commandTimeout.TotalSeconds);

            if (parameters != null)
                foreach (var param in parameters)
                    command.Parameters.Add(param);

            CallProviderSpecificCommandSettings(command);

            return command;
        }

        /// <summary>
        /// Adjusts the CommandBuilder by provider.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        protected virtual void CallProviderSpecificCommandSettings(DbCommand command)
        {
            _providerSpecificCommandSettings?.Invoke(command);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        protected virtual void OpenConnectionInternal()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Connection.Close();
                _connection.Dispose();
            }
        }

        #endregion IDisposable Members
    }

#if !NETFX40

    /// <summary>
    /// Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public partial class DatabaseManager
    {
        /// <summary>
        /// Executes the data reader asynchronously.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual async Task<IDataReader> ExecuteDataReaderAsync(string commandText,
            CommandType commandType,
            TimeSpan commandTimeout,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            IDataReader results = null;
            var open = _explicitlyOpened;
            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {

                    // If the connection was open before execute was called, then do not automatically close connection.
                    results = await (open
                        ? command.ExecuteReaderAsync(cancellationToken)
                        : command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken));
                }
            }
            catch
            {
                if (results != null && !results.IsClosed)
                    results.Close();

                throw;
            }

            return results;
        }

        /// <summary>
        /// Executes the stored procedure asynchronously.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText,
            CommandType commandType,
            TimeSpan commandTimeout,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            var open = _explicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {
                    return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                if (Transaction.Current == null && !open)
                    CloseConnection();
            }
        }

        /// <summary>
        /// Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The CommandBuilder text.</param>
        /// <param name="commandType">Type of the CommandBuilder.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        public async Task<T> ExecuteScalarAsync<T>(string commandText,
            CommandType commandType,
            TimeSpan commandTimeout,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException(nameof(commandText));

            var open = _explicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, commandTimeout, parameters))
                {
                    return
                        (T)
                            CastValue(typeof(T),
                                await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
                }
            }
            finally
            {
                if (Transaction.Current == null && !open)
                    CloseConnection();
            }
        }
    }

#endif
}
