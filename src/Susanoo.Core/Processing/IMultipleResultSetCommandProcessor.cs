using System;
using System.Data.Common;
using Susanoo.Command;
using Susanoo.Exceptions;

namespace Susanoo.Processing
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface IMultipleResultSetCommandProcessor<TFilter>
        : ICommandProcessorWithResults<TFilter>,
#if !NETFX40
            IMultipleResultSetCommandProcessorAsync<TFilter>,
#endif
            ICommandProcessor<TFilter>
    {

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>IMultipleResultSetCommandProcessor&lt;TFilter&gt;.</returns>
        IMultipleResultSetCommandProcessor<TFilter> InterceptOrProxyWith(
            Func<IMultipleResultSetCommandProcessor<TFilter>, IMultipleResultSetCommandProcessor<TFilter>> interceptOrProxy);


        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>IResultSetReader.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        IResultSetReader Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo);

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        IResultSetReader Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters);

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IResultSetReader.</returns>
        /// <exception cref="SusanooExecutionException">Any exception occured during execution.</exception>
        IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters);
    }
}