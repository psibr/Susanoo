#region

using System.Data.Common;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Susanoo
{
    /// <summary>
    /// A fully built and ready to be executed command expression with a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public partial class NoResultSetCommandProcessor<TFilter> : CommandProcessorCommon, ICommandProcessor<TFilter>
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetCommandProcessor{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name of the processor.</param>
        public NoResultSetCommandProcessor(ICommandExpression<TFilter> command, string name)
        {
            _commandExpression = command;

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get { return _commandExpression.CacheHash; }
        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return databaseManager.ExecuteScalar<TReturn>(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        ///     Executes the scalar.
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
        ///     Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return databaseManager.ExecuteNonQuery(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        ///     Executes the non query.
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
    ///     A fully built and ready to be executed command expression with a filter parameter.
    /// </summary>
    public partial class NoResultSetCommandProcessor<TFilter>
    {
        /// <summary>
        ///     Execute scalar as an asynchronous operation.
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
            return await databaseManager.ExecuteScalarAsync<TReturn>(CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                cancellationToken,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters))
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager, TFilter filter,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteNonQueryAsync(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                cancellationToken,
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        ///     Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteNonQueryAsync(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                default(CancellationToken),
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters));
        }

        /// <summary>
        ///     Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteNonQueryAsync(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                cancellationToken,
                explicitParameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Executes the non query async.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteNonQueryAsync(
                CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                default(CancellationToken),
                explicitParameters).ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteScalarAsync<TReturn>(CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                cancellationToken,
                explicitParameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteScalarAsync<TReturn>(CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                default(CancellationToken),
                CommandExpression.BuildParameters(databaseManager, filter, explicitParameters))
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return await databaseManager.ExecuteScalarAsync<TReturn>(CommandExpression.CommandText,
                CommandExpression.DbCommandType,
                default(CancellationToken),
                explicitParameters)
                .ConfigureAwait(false);
        }
    }

#endif
}