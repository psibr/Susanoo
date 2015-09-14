#region

using System;
using System.Data.Common;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Susanoo.Processing;

#endregion

namespace Susanoo.Command
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public partial class NoResultSetCommandProcessor<TFilter> : 
        ICommandProcessor<TFilter>
    {
        private readonly ICommandBuilderInfo<TFilter> _commandBuilderInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetCommandProcessor{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public NoResultSetCommandProcessor(ICommandBuilderInfo<TFilter> command)
        {
            _commandBuilderInfo = command;
        }

        /// <summary>
        /// Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public ICommandBuilderInfo<TFilter> CommandBuilderInfo => _commandBuilderInfo;

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash => _commandBuilderInfo.CacheHash;

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            var result = default(TReturn);
            DbParameter[] parameters = null;
            try
            {
                parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);
                result = databaseManager.ExecuteScalar<TReturn>(
                    CommandBuilderInfo.CommandText,
                    CommandBuilderInfo.DbCommandType,
                    parameters);
            }
            catch (Exception ex)
            {
                CommandManager.Instance.HandleExecutionException(CommandBuilderInfo, ex, parameters);
            }

            return result;

        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return ExecuteScalar<TReturn>(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return ExecuteScalar<TReturn>(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            var result = default(int);
            DbParameter[] parameters = null;
            try
            {
                parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, null, explicitParameters);
                result = databaseManager.ExecuteNonQuery(
                    CommandBuilderInfo.CommandText,
                    CommandBuilderInfo.DbCommandType,
                    parameters);
            }
            catch (Exception ex)
            {
                CommandManager.Instance.HandleExecutionException(CommandBuilderInfo, ex, parameters);
            }

            return result;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return ExecuteNonQuery(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return ExecuteNonQuery(databaseManager, default(TFilter), explicitParameters);
        }
    }

#if !NETFX40

    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with a filter parameter.
    /// </summary>
    public partial class NoResultSetCommandProcessor<TFilter>
    {
        /// <summary>
        /// Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            var result = default(TReturn);
            DbParameter[] parameters = null;
            try
            {
                parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, null, explicitParameters);
                result = await databaseManager.ExecuteScalarAsync<TReturn>(
                    CommandBuilderInfo.CommandText,
                    CommandBuilderInfo.DbCommandType,
                    cancellationToken,
                    parameters)
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                CommandManager.Instance.HandleExecutionException(CommandBuilderInfo, ex, parameters);
            }

            return result;
        }

        /// <summary>
        /// Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager, TFilter filter,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            var result = default(int);
            DbParameter[] parameters = null;
            try
            {
                parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, null, explicitParameters);
                result = await databaseManager.ExecuteNonQueryAsync(
                    CommandBuilderInfo.CommandText,
                    CommandBuilderInfo.DbCommandType,
                    cancellationToken,
                    parameters)
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                CommandManager.Instance.HandleExecutionException(CommandBuilderInfo, ex, parameters);
            }

            return result;
        }

        /// <summary>
        /// Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return await ExecuteNonQueryAsync(databaseManager, filter, default(CancellationToken), explicitParameters);
        }

        /// <summary>
        /// Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await ExecuteNonQueryAsync(databaseManager, default(TFilter), cancellationToken, explicitParameters);
        }

        /// <summary>
        /// Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return await ExecuteNonQueryAsync(databaseManager, default(TFilter), default(CancellationToken), explicitParameters);
        }

        /// <summary>
        /// Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, default(TFilter), cancellationToken, explicitParameters);
        }

        /// <summary>
        /// Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, filter, default(CancellationToken), explicitParameters);
        }

        /// <summary>
        /// Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, default(TFilter), default(CancellationToken), explicitParameters);
        }
    }

#endif
}