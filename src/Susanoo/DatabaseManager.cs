using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace Susanoo
{
    /// <summary>
    /// Standard Database Manager for Susanoo that supports any DB implementation that provides a DbProviderFactory.
    /// </summary>
    public class DatabaseManager : IDatabaseManager, IDisposable
    {
        private readonly string _ConnectionString;
        private IDbConnection _Connection;
        private readonly Action<IDbCommand> providerSpecificCommandSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="providerSpecificCommandSettings">The provider specific command settings.</param>
        /// <exception cref="System.NotSupportedException">The database provider type specified is not supported. Provider:  + provider.ToString()</exception>
        public DatabaseManager(DbProviderFactory provider, string connectionStringName, Action<IDbCommand> providerSpecificCommandSettings)
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
        protected IDbConnection Connection
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
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType, IDbTransaction transaction, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;

            if (transaction != null)
            {
                try
                {
                    this.OpenConnection();

                    using (IDbCommand command = this.Provider.CreateCommand())
                    {
                        this.CallProviderSpecificCommandSettings(command);

                        command.CommandType = commandType;
                        command.Connection = this.Connection;

                        command.Transaction = transaction;

                        command.CommandText = commandText;

                        parameters.ToList().ForEach(parameter => command.Parameters.Add(parameter));

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
        /// Adjusts the command by provider.
        /// </summary>
        /// <param name="command">The command.</param>
        protected virtual void CallProviderSpecificCommandSettings(IDbCommand command)
        {
            if (this.providerSpecificCommandSettings != null)
                this.providerSpecificCommandSettings(command);
        }

        /// <summary>
        /// Executes the data reader.
        /// </summary>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">commandText</exception>
        public virtual IDataReader ExecuteDataReader(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            IDataReader results = null;

            try
            {
                this.OpenConnection();

                using (IDbCommand command = this.Provider.CreateCommand())
                {
                    this.CallProviderSpecificCommandSettings(command);

                    command.CommandType = commandType;
                    command.Connection = this.Connection;

                    command.CommandText = commandText;

                    parameters.ToList().ForEach(parameter => command.Parameters.Add(parameter));

                    results = command.ExecuteReader(CommandBehavior.CloseConnection);
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
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, IDbTransaction transaction, params IDataParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            try
            {
                this.OpenConnection();

                using (IDbCommand command = this.Provider.CreateCommand())
                {
                    this.CallProviderSpecificCommandSettings(command);

                    command.CommandType = System.Data.CommandType.Text;
                    command.Connection = this.Connection;

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = commandText;

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
        /// Detects if a value is DBNull, null, or has value.
        /// </summary>
        /// <param name="newType">The new type.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="typeName">Name of the type from the database (used for date/time to string conversion).</param>
        /// <returns>Value as type T if value is not DBNull, null, or invalid cast; otherwise defaultValue.</returns>
        public static object CastValue(Type newType, object value, object defaultValue, string typeName)
        {
            object returnValue;

            if (value is DBNull || value == null)
                returnValue = defaultValue;
            else if (newType == typeof(bool) && (value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32)))
                returnValue = ((object)(int.Parse(value.ToString(), CultureInfo.InvariantCulture) > 0 ? true : false));
            else if (newType == typeof(int) && value.GetType() == typeof(long))
                returnValue = ((object)((int)((long)value)));
            else if (newType == typeof(int) && value.GetType() == typeof(decimal))
                returnValue = ((object)((int)((decimal)value)));
            else if (newType == typeof(string))
            {
                returnValue = value.ToString();

                if (!string.IsNullOrEmpty(typeName))
                    if (typeName == "date")
                        returnValue = ((DateTime)value).ToString("MM/dd/yyyy", CultureInfo.CurrentCulture);
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
        public static object CastValue(Type newType, object value, object defaultValue)
        {
            return CastValue(newType, value, defaultValue, null);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public virtual T ExecuteScalar<T>(string commandText, CommandType commandType, params IDataParameter[] parameters)
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
        public virtual int ExecuteStoredProcedureNonQuery(string commandText, CommandType commandType, IDbTransaction transaction, params IDbDataParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText");

            try
            {
                this.OpenConnection();

                using (IDbCommand command = this.Provider.CreateCommand())
                {
                    this.CallProviderSpecificCommandSettings(command);

                    command.CommandType = commandType;
                    command.Connection = this.Connection;

                    if (transaction != null)
                        command.Transaction = transaction;

                    command.CommandText = commandText;

                    parameters.ToList()
                              .ForEach(parameter => command.Parameters.Add(parameter));

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
        public virtual int ExecuteStoredProcedureNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] parameters)
        {
            return this.ExecuteStoredProcedureNonQuery(commandText, commandType, null, parameters);
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <returns></returns>
        public virtual IDbDataParameter CreateParameter()
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
        public virtual IDbDataParameter CreateParameter(string parameterName, ParameterDirection parameterDirection, DbType parameterType, object value)
        {
            IDbDataParameter newParam = this.Provider.CreateParameter();
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
        public virtual IDbDataParameter CreateInputParameter(string parameterName, DbType parameterType, object value)
        {
            return this.CreateParameter(parameterName, ParameterDirection.Input, parameterType, value);
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        public virtual IDbTransaction BeginTransaction()
        {
            this.OpenConnection();
            return this.Connection.BeginTransaction();
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        protected void OpenConnection()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        protected void CloseConnection()
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
    }
}