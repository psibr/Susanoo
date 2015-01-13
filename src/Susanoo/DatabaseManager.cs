#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public partial class DatabaseManager : IDatabaseManager, IDisposable
    {
        private DbConnection _connection;
        private bool ExplicitlyOpened;
        private readonly string _connectionString;
        private readonly Action<DbCommand> _providerSpecificCommandSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerSpecificCommandSettings">The provider specific command settings.</param>
        /// <exception cref="System.NotSupportedException">
        ///     The database provider type specified is not supported. Provider:  +
        ///     provider.ToString()
        /// </exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName,
            Action<DbCommand> providerSpecificCommandSettings)
            : this(provider, connectionStringName)
        {
            _providerSpecificCommandSettings = providerSpecificCommandSettings;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.NotSupportedException">
        ///     The database provider type specified is not supported. Provider:  +
        ///     provider.ToString()
        /// </exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName)
        {
            Provider = provider;

            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.ArgumentException">
        ///     Provider is a required component of the connection
        ///     string.;connectionStringName
        /// </exception>
        public DatabaseManager(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            Provider =
                DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName);

            if (Provider == null)
                throw new ArgumentException("Provider is a required component of the connection string.",
                    "connectionStringName");
        }

        /// <summary>
        ///     Gets the connection.
        /// </summary>
        /// <value>
        ///     The connection.
        /// </value>
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
        ///     Gets the provider.
        /// </summary>
        /// <value>
        ///     The provider.
        /// </value>
        public DbProviderFactory Provider { get; private set; }

        /// <summary>
        ///     Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;

            try
            {
                var open = ExplicitlyOpened;
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, parameters))
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
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A single value of type T.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            var open = ExplicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (DbCommand command = PrepCommand(Connection, commandText, commandType, parameters))
                {
                    object result = CastValue(typeof (T), command.ExecuteScalar());

                    return (T) result;
                }
            }
            finally
            {
                if (Transaction.Current == null && !open)
                    CloseConnection();
            }
        }

        /// <summary>
        ///     Executes the stored procedure.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            var open = (Connection.State != ConnectionState.Closed);

            try
            {
                OpenConnectionInternal();

                using (DbCommand command = PrepCommand(Connection, commandText, commandType, parameters))
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
        ///     Creates a parameter.
        /// </summary>
        /// <returns></returns>
        public virtual DbParameter CreateParameter()
        {
            return Provider.CreateParameter();
        }

        /// <summary>
        ///     Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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
        ///     Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DbParameter CreateInputParameter(string parameterName, DbType parameterType, object value)
        {
            return CreateParameter(parameterName, ParameterDirection.Input, parameterType, value);
        }

        /// <summary>
        ///     Opens the connection.
        /// </summary>
        public virtual void OpenConnection()
        {
            ExplicitlyOpened = true;

            OpenConnectionInternal();
        }

        /// <summary>
        ///     Closes the connection.
        /// </summary>
        public virtual void CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                ExplicitlyOpened = false;
            }
        }

        /// <summary>
        ///     Gets the state of the connection.
        /// </summary>
        /// <value>The state.</value>
        public ConnectionState State
        {
            get { return Connection.State; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs a bulk copy operation.
        /// </summary>
        /// <typeparam name="TRecord">The type of the record.</typeparam>
        /// <param name="destinationTableName">Name of the destination table.</param>
        /// <param name="records">The records.</param>
        /// <param name="whiteList">The white list of properties to include. Default is NULL.</param>
        /// <param name="blackList">The black list of properties to exclude. Default is NULL.</param>
        public virtual void BulkCopy<TRecord>(string destinationTableName,
            IEnumerable<TRecord> records,
            IEnumerable<string> whiteList = null,
            IEnumerable<string> blackList = null)
        {
            //TODO: Implement BulkCopy operation.
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Realizes an instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        ~DatabaseManager()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Returns value or it's string representation.
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

            if (newType == typeof (string))
            {
                returnValue = (value ?? "").ToString();
            }

            return returnValue;
        }

        /// <summary>
        ///     Begins a transaction.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Prefer using System.Transactions.TransactionScope.", false)]
        public virtual DbTransaction BeginTransaction()
        {
            OpenConnectionInternal();
            return Connection.BeginTransaction();
        }

        /// <summary>
        ///     Preps the command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>DbCommand.</returns>
        protected virtual DbCommand PrepCommand(DbConnection connection, string commandText, CommandType commandType,
            params DbParameter[] parameters)
        {
            DbCommand command = Provider.CreateCommand();

            if (command != null)
            {
                command.CommandType = commandType;
                command.Connection = Connection;

                command.CommandText = commandText;

                if (parameters != null)
                    foreach (var param in parameters)
                        command.Parameters.Add(param);

                CallProviderSpecificCommandSettings(command);
            }

            return command;
        }

        /// <summary>
        ///     Adjusts the command by provider.
        /// </summary>
        /// <param name="command">The command.</param>
        protected virtual void CallProviderSpecificCommandSettings(DbCommand command)
        {
            if (_providerSpecificCommandSettings != null)
                _providerSpecificCommandSettings(command);
        }

        /// <summary>
        ///     Opens the connection.
        /// </summary>
        protected virtual void OpenConnectionInternal()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
        }

        #region IDisposable Members

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
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
    ///     Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public partial class DatabaseManager : IDatabaseManager, IDisposable
    {
        /// <summary>
        ///     Executes the data reader asynchronously.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDataReader.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual async Task<IDataReader> ExecuteDataReaderAsync(string commandText,
            CommandType commandType,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;
            var open = ExplicitlyOpened;
            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, parameters))
                {
                    if (open)
                        results = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                    else
                        results =
                            await
                                command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken)
                                    .ConfigureAwait(false);
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
        ///     Executes the stored procedure asynchronously.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual async Task<int> ExecuteNonQueryAsync(string commandText,
            CommandType commandType,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            var open = ExplicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, parameters))
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
        ///     execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        public async Task<T> ExecuteScalarAsync<T>(string commandText,
            CommandType commandType,
            CancellationToken cancellationToken = default(CancellationToken),
            params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            var open = ExplicitlyOpened;

            try
            {
                OpenConnectionInternal();

                using (var command = PrepCommand(Connection, commandText, commandType, parameters))
                {
                    return
                        (T)
                            CastValue(typeof (T),
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