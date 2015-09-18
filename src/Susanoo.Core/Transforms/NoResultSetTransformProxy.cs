using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Susanoo.Command;
using Susanoo.Processing;

namespace Susanoo.Transforms
{
    /// <summary>
    /// A proxy for no result set command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class NoResultSetTransformProxy<TFilter>
        : ICommandProcessor<TFilter>
    {
        private readonly ICommandProcessor<TFilter> _source;
        private readonly IEnumerable<CommandTransform> _transforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoResultSetTransformProxy{TFilter}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        public NoResultSetTransformProxy(ICommandProcessor<TFilter> source, IEnumerable<CommandTransform> transforms)
        {
            _source = source;
            _transforms = transforms;
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash =>
            _source.CacheHash;

        /// <summary>
        /// Gets the CommandBuilder information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        public ICommandBuilderInfo<TFilter> CommandBuilderInfo =>
            _source.CommandBuilderInfo;

        /// <summary>
        /// Gets or sets the timeout of a command execution.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>ICommandProcessor&lt;TFilter&gt;.</returns>
        public ICommandProcessor<TFilter> InterceptOrProxyWith(Func<ICommandProcessor<TFilter>, ICommandProcessor<TFilter>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

        /// <summary>
        ///     Executes the non query.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return _source.ExecuteNonQuery(databaseManager, transformed);
        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return _source.ExecuteScalar<TReturn>(databaseManager, transformed);
        }

        /// <summary>
        ///     Executes the scalar.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        public TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter,
            object parameterObject, params DbParameter[] explicitParameters)
        {
            var executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo
                    .BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return ExecuteScalar<TReturn>(databaseManager, executableCommandInfo);
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
            return ExecuteScalar<TReturn>(databaseManager, filter, null, explicitParameters);
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
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            var executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo
                    .BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return ExecuteNonQuery(databaseManager, executableCommandInfo);
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
            return ExecuteNonQuery(databaseManager, filter, null, explicitParameters);
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

#if !NETFX40

        /// <summary>
        ///     Executes the non query asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return await _source.ExecuteNonQueryAsync(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Executes the scalar asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return await _source.ExecuteScalarAsync<TReturn>(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters)
        {
            var executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo
                    .BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return await ExecuteScalarAsync<TReturn>(databaseManager, executableCommandInfo, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Executes the non query asynchronous.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Int32&gt;.</returns>
        public async Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager, TFilter filter,
            object parameterObject,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            var executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo
                    .BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return await ExecuteNonQueryAsync(databaseManager, executableCommandInfo, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return
                await ExecuteScalarAsync<TReturn>(databaseManager, filter, null, cancellationToken, explicitParameters)
                    .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, default(TFilter), null, CancellationToken.None,
                        explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, filter, null, CancellationToken.None,
                        explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, default(TFilter), null, CancellationToken.None,
                        explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        ///     Execute scalar as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        public async Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            return
                await
                    ExecuteScalarAsync<TReturn>(databaseManager, filter, parameterObject, CancellationToken.None,
                        explicitParameters)
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
            return
                await ExecuteNonQueryAsync(databaseManager, filter, null, cancellationToken, explicitParameters)
                    .ConfigureAwait(false);
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
            return await ExecuteNonQueryAsync(databaseManager, filter, default(CancellationToken), explicitParameters)
                .ConfigureAwait(false);
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
            return
                await ExecuteNonQueryAsync(databaseManager, default(TFilter), cancellationToken, explicitParameters)
                    .ConfigureAwait(false);
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
            return
                await
                    ExecuteNonQueryAsync(databaseManager, default(TFilter), default(CancellationToken),
                        explicitParameters)
                        .ConfigureAwait(false);
        }

#endif

    }
}
