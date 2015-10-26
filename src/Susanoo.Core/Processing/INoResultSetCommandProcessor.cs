#region

using Susanoo.Command;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    /// Represents a fully built and ready to be executed CommandBuilder expression with no mapping expressions and a filter
    /// parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface INoResultCommandProcessor<TFilter> : ICommandProcessor<TFilter>
#if !NETFX40
        , INoResultSetCommandProcessorAsync<TFilter>
#endif
    {
        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter&gt;.</returns>
        INoResultCommandProcessor<TFilter> InterceptOrProxyWith(
            Func<INoResultCommandProcessor<TFilter>, INoResultCommandProcessor<TFilter>> interceptOrProxy);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, TFilter filter,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the command and retrieves a single value.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TReturn.</returns>
        TReturn ExecuteScalar<TReturn>(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        int ExecuteNonQuery(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);
    }

#if !NETFX40
    /// <summary>
    /// Async actions for no result CommandBuilder processors.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface INoResultSetCommandProcessorAsync<in TFilter>
    {
        /// <summary>
        /// Executes the scalar asynchronous.
        /// </summary>
        /// <typeparam name="TReturn">The type of the t return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the non query asynchronous.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder and returns a return code asynchronously.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Int32.</returns>
        Task<int> ExecuteNonQueryAsync(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>TReturn.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            IExecutableCommandInfo executableCommandInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the scalar action asynchronously.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;TReturn&gt;.</returns>
        Task<TReturn> ExecuteScalarAsync<TReturn>(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters);
    }

#endif
}