using Susanoo.Command;
using Susanoo.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Susanoo.Processing
{
    /// <summary>
    /// Defines the standard flow and structure of a Single Result Set command processor.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class SingleResultSetCommandProcessorStructure<TFilter, TResult>
        : CommandProcessorWithResults<TFilter>
    {
        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public abstract ISingleResultSetCommandProcessor<TFilter, TResult> InterceptOrProxyWith(
            Func<ISingleResultSetCommandProcessor<TFilter, TResult>, ISingleResultSetCommandProcessor<TFilter, TResult>>
                interceptOrProxy);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            IExecutableCommandInfo executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return Execute(databaseManager, executableCommandInfo);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;TResult&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public abstract IEnumerable<TResult> Execute(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

#if !NETFX40

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public abstract Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken, params DbParameter[] explicitParameters)

        {
            IExecutableCommandInfo executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return await 
                ExecuteAsync(databaseManager, executableCommandInfo, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await
                    ExecuteAsync(databaseManager, default(TFilter), null, cancellationToken, explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return await
                    ExecuteAsync(databaseManager, default(TFilter), null, default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            return await
                    ExecuteAsync(databaseManager, filter, parameterObject, default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            return await
                    ExecuteAsync(databaseManager, filter, null, default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="SusanooExecutionException">Any exception is encountered during execution.</exception>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            return await
                    ExecuteAsync(databaseManager, filter, null, default(CancellationToken), explicitParameters)
                        .ConfigureAwait(false);
        }

#endif
    }
}