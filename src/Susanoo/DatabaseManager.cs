﻿using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Susanoo
{
    /// <summary>
    /// Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public class DatabaseManager : IDatabaseManager, IDisposable
    {
        private readonly string _ConnectionString;
        private readonly Action<DbCommand> providerSpecificCommandSettings;
        private DbConnection _Connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerSpecificCommandSettings">The provider specific command settings.</param>
        /// <exception cref="System.NotSupportedException">The database provider type specified is not supported. Provider:  + provider.ToString()</exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName, Action<DbCommand> providerSpecificCommandSettings)
            : this(provider, connectionStringName)
        {
            this.providerSpecificCommandSettings = providerSpecificCommandSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.NotSupportedException">The database provider type specified is not supported. Provider:  + provider.ToString()</exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName)
        {
            this.Provider = provider;

            this._ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DatabaseManager(string connectionStringName)
        {
            this._ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                .ConnectionString;

            this.Provider = DbProviderFactories.GetFactory(this.Connection);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        ~DatabaseManager()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        protected DbConnection Connection
        {
            get
            {
                if (this._Connection == null)
                {
                    this._Connection = this.Provider.CreateConnection();
                    this._Connection.ConnectionString = this._ConnectionString;
                }

                return this._Connection;
            }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        protected DbProviderFactory Provider { get; private set; }

        /// <summary>
        /// Detects if a value is DBNull, null, or has value.
        /// </summary>
        /// <param name="newType">The new type.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="typeName">Name of the type from the database (used for date/time to string conversion).</param>
        /// <returns>Value as type T if value is not DBNull, null, or invalid cast; otherwise defaultValue.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CastValue(Type newType, object value, object defaultValue, string typeName)
        {
            object returnValue;

            //else if (newType == typeof(bool) && (value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32)))
            //    returnValue = ((object)(int.Parse(value.ToString(), CultureInfo.InvariantCulture) > 0 ? true : false));
            //else if (newType == typeof(int) && value.GetType() == typeof(long))
            //    returnValue = ((object)((int)((long)value)));
            //else if (newType == typeof(int) && value.GetType() == typeof(decimal))
            //    returnValue = ((object)((int)((decimal)value)));
            if (newType == typeof(string))
            {
                returnValue = value.ToString();

                //if (!string.IsNullOrEmpty(typeName))
                //    if (typeName == "date")
                //        returnValue = ((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
            }
            else
                returnValue = value;

            return returnValue;
        }

        /// <summary>
        /// Casts the value.
        /// </summary>
        /// <param name="newType">The new type.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CastValue(Type newType, object value, object defaultValue)
        {
            return CastValue(newType, value, defaultValue, null);
        }

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType, DbTransaction transaction, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;

            if (transaction != null)
            {
                try
                {
                    this.OpenConnection();

                    using (DbCommand command = this.Provider.CreateCommand())
                    {
                        command.CommandType = commandType;
                        command.Connection = this.Connection;

                        command.Transaction = transaction;

                        command.CommandText = commandText;

                        if (parameters != null)
                        parameters.ToList().ForEach(parameter => command.Parameters.Add(parameter));

                        this.CallProviderSpecificCommandSettings(command);

                        results = command.ExecuteReader();
                    }
                }
                catch
                {
                    if (results != null && !results.IsClosed)
                        results.Close();

                    throw;
                }
            }
            else
                results = this.ExecuteDataReader(commandText, commandType, parameters);

            return results;
        }

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;

            try
            {
                this.OpenConnection();

                using (DbCommand command = this.Provider.CreateCommand())
                {
                    command.CommandType = commandType;
                    command.Connection = this.Connection;

                    command.CommandText = commandText;

                    if (parameters != null)
                        foreach (var item in parameters)
                        {
                            command.Parameters.Add(item);
                        }

                    this.CallProviderSpecificCommandSettings(command);

                    results = command.ExecuteReader();
                }

                return results;
            }
            catch
            {
                if (results != null && !results.IsClosed)
                    results.Close();

                throw;
            }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType"></param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, DbTransaction transaction, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            try
            {
                this.OpenConnection();

                using (DbCommand command = this.Provider.CreateCommand())
                {
                    this.CallProviderSpecificCommandSettings(command);

                    command.CommandType = commandType;
                    command.Connection = this.Connection;

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = commandText;

                    if (parameters != null)
                        parameters.ToList().ForEach(parameter => command.Parameters.Add(parameter));

                    return (T)DatabaseManager.CastValue(typeof(T), command.ExecuteScalar(), default(T));
                }
            }
            finally
            {
                if (transaction == null)
                    this.CloseConnection();
            }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            return this.ExecuteScalar<T>(commandText, commandType, null, parameters);
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, DbTransaction transaction, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            try
            {
                this.OpenConnection();

                using (DbCommand command = this.Provider.CreateCommand())
                {
                    command.CommandType = commandType;
                    command.Connection = this.Connection;

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = commandText;

                    if (parameters != null)
                    parameters.ToList()
                              .ForEach(parameter => command.Parameters.Add(parameter));

                    this.CallProviderSpecificCommandSettings(command);

                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (transaction == null)
                    this.CloseConnection();
            }
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType"></param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            return this.ExecuteNonQuery(commandText, commandType, null, parameters);
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns></returns>
        public virtual DbParameter CreateParameter()
        {
            return this.Provider.CreateParameter();
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string parameterName, ParameterDirection parameterDirection, DbType parameterType, object value)
        {
            DbParameter newParam = this.Provider.CreateParameter();
            newParam.ParameterName = parameterName;
            newParam.Direction = parameterDirection;
            newParam.DbType = parameterType;
            newParam.Value = value;
            return newParam;
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DbParameter CreateInputParameter(string parameterName, DbType parameterType, object value)
        {
            return this.CreateParameter(parameterName, ParameterDirection.Input, parameterType, value);
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        public virtual DbTransaction BeginTransaction()
        {
            this.OpenConnection();
            return this.Connection.BeginTransaction();
        }

        /// <summary>
        /// Adjusts the command by provider.
        /// </summary>
        /// <param name="command">The command.</param>
        protected virtual void CallProviderSpecificCommandSettings(DbCommand command)
        {
            if (this.providerSpecificCommandSettings != null)
                this.providerSpecificCommandSettings(command);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        protected virtual void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        protected virtual void CloseConnection()
        {
            if (this.Connection.State != ConnectionState.Closed)
                this.Connection.Close();
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.Connection.Close();
                this._Connection.Dispose();
            }
        }

        #endregion IDisposable Members

        /// <summary>
        /// execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        public async System.Threading.Tasks.Task<T> ExecuteScalarAsync<T>(string commandText, CommandType commandType, DbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken), params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            try
            {
                this.OpenConnection();

                using (DbCommand command = this.Provider.CreateCommand())
                {
                    this.CallProviderSpecificCommandSettings(command);

                    command.CommandType = System.Data.CommandType.Text;
                    command.Connection = this.Connection;

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = commandText;

                    if (parameters != null)
                    parameters.ToList().ForEach(parameter => command.Parameters.Add(parameter));

                    return (T)DatabaseManager.CastValue(typeof(T), await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false), default(T));
                }
            }
            finally
            {
                if (transaction == null)
                    this.CloseConnection();
            }
        }

        /// <summary>
        /// execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public async System.Threading.Tasks.Task<T> ExecuteScalarAsync<T>(string commandText, CommandType commandType, CancellationToken cancellationToken = default(CancellationToken), params DbParameter[] parameters)
        {
            return await ExecuteScalarAsync<T>(commandText, commandType, null, cancellationToken, parameters);
        }
    }
}